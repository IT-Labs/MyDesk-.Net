using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OpenIdConfigurationKeysFactory : IOpenIdConfigurationKeysFactory
    {
        private const string azureCacheKey = "Azure security keys";
        private const string googleCacheKey = "Google security keys";

        private readonly IApplicationParmeters _applicationParmeters;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public OpenIdConfigurationKeysFactory(IApplicationParmeters applicationParmeters,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _applicationParmeters = applicationParmeters;
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
        }

        public IEnumerable<SecurityKey> GetKeys(AuthTypes authType)
        {
            if (authType == AuthTypes.Azure)
            {
                if (_cache.TryGetValue(azureCacheKey, out IEnumerable<SecurityKey> cachedSecurityKeys))
                {
                    return cachedSecurityKeys;
                }
                else
                {
                    string metadataAddress = _applicationParmeters.GetMetadataAddress();

                    if (!string.IsNullOrEmpty(metadataAddress))
                    {
                        IConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                                new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever());
                        OpenIdConnectConfiguration openIdConnectConfiguration = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;

                        ICollection<SecurityKey> securityKeys = openIdConnectConfiguration.SigningKeys;
                        _cache.Set(azureCacheKey, securityKeys, GetCacheOptions());

                        return securityKeys;
                    }
                }
            }
            else if (authType == AuthTypes.Google)
            {
                if (_cache.TryGetValue(googleCacheKey, out IEnumerable<SecurityKey> cachedSecurityKeys))
                {
                    return cachedSecurityKeys;
                }
                else
                {
                    HttpRequestMessage request = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://www.googleapis.com/oauth2/v1/certs")
                    };

                    HttpResponseMessage response = _httpClient.Send(request, CancellationToken.None);

                    if (response.IsSuccessStatusCode)
                    {
                        string stringResponse = response.Content.ReadAsStringAsync().Result;
                        Dictionary<string, string> dictionaryKeys = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringResponse);

                        IEnumerable<SecurityKey> securityKeys = dictionaryKeys.Select(x => new X509SecurityKey(new X509Certificate2(Encoding.UTF8.GetBytes(x.Value))));

                        _cache.Set(googleCacheKey, securityKeys, GetCacheOptions());

                        return securityKeys;
                    }
                }
            }

            return null;
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(2))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(8))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(2);
        }
    }
}

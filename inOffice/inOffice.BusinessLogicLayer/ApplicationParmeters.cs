using inOfficeApplication.Data.Interfaces.BusinessLogic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace inOffice.BusinessLogicLayer
{
    public class ApplicationParmeters : IApplicationParmeters
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public ApplicationParmeters(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public string GetAdminEmail()
        {
            return GetConfigurationProperty("GetAdminEmail cache key", "AdminEmail");
        }

        public string GetAdminPassword()
        {
            return GetConfigurationProperty("GetAdminPassword cache key", "AdminPassword");
        }

        public string GetConnectionString()
        {
            return GetConfigurationProperty("GetConnectionString cache key", "ConnectionString");
        }

        public string GetSentimentEndpoint()
        {
            return GetConfigurationProperty("GetSentimentEndpoint cache key", "SentimentEndpoint");
        }

        public string GetTenantClaimKey()
        {
            return GetConfigurationProperty("GetTenantClaimKey cache key", "TenantClaimKey");
        }

        public Dictionary<string, string> GetTenants()
        {
            string tenantsCacheKey = "GetTenants cache key";

            if (_cache.TryGetValue(tenantsCacheKey, out Dictionary<string, string> cachedTenants))
            {
                return cachedTenants;
            }
            else
            {
                Dictionary<string, string> tenantsData;
                string tenants = _configuration["Tenants"];

                if (!string.IsNullOrEmpty(tenants))
                {
                    tenantsData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tenants);
                }
                else
                {
                    tenantsData = new Dictionary<string, string>();
                }

                _cache.Set(tenantsCacheKey, tenantsData, GetCacheOptions());
                return tenantsData;
            }
        }

        public string GetCustomBearerTokenSigningKey(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:Local:CustomBearerTokenSigningKey" : "Authentication_Local_CustomBearerTokenSigningKey";
            return GetConfigurationProperty("GetCustomBearerTokenSigningKey cache key", configurationKey);
        }

        public string GetAzureAdIssuer(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:AzureAd:Issuer" : "Authentication_AzureAd_Issuer";
            return GetConfigurationProperty("GetAzureAdIssuer cache key", configurationKey);
        }

        public string GetAzureAdAudience(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:AzureAd:Audience" : "Authentication_AzureAd_Audience";
            return GetConfigurationProperty("GetAzureAdAudience cache key", configurationKey);
        }

        public string GetAzureAdMetadataAddress(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:AzureAd:MetadataAddress" : "Authentication_AzureAd_MetadataAddress";
            return GetConfigurationProperty("GetAzureAdMetadataAddress cache key", configurationKey);
        }

        public string GetGoogleIssuer(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:Google:Issuer" : "Authentication_Google_Issuer";
            return GetConfigurationProperty("GetGoogleIssuer cache key", configurationKey);
        }

        public string GetGoogleClientId(bool isDevelopment)
        {
            var configurationKey = isDevelopment ? "Authentication:Google:ClientId" : "Authentication_Google_ClientId";
            return GetConfigurationProperty("GetGoogleClientId cache key", configurationKey);
        }

        private string GetConfigurationProperty(string cacheKey, string configurationKey)
        {
            if (_cache.TryGetValue(cacheKey, out string cachedValue))
            {
                return cachedValue;
            }
            else
            {
                string configurationValue = _configuration[configurationKey];
                _cache.Set(cacheKey, configurationValue, GetCacheOptions());

                return configurationValue;
            }
        }

        private MemoryCacheEntryOptions GetCacheOptions()
        {
            return new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(24))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(2);
        }
    }
}

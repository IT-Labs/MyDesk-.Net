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

        public string GetJwtIssuer()
        {
            return GetConfigurationProperty("GetJwtIssuer cache key", "JwtIssuer");
        }

        public string GetJwtAudience()
        {
            return GetConfigurationProperty("GetJwtAudience cache key", "JwtAudience");
        }

        public string GetMetadataAddress()
        {
            return GetConfigurationProperty("GetMetadataAddress cache key", "MetadataAddress");
        }

        public string GetSentimentEndpoint()
        {
            return GetConfigurationProperty("GetSentimentEndpoint cache key", "SentimentEndpoint");
        }

        public string GetCustomBearerTokenSigningKey()
        {
            return GetConfigurationProperty("GetCustomBearerTokenSigningKey cache key", "CustomBearerTokenSigningKey");
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

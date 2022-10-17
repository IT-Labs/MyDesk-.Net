using inOfficeApplication.Data.Interfaces.BusinessLogic;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace inOffice.BusinessLogicLayer
{
    public class ApplicationParmeters : IApplicationParmeters
    {
        private readonly IConfiguration _configuration;
        public ApplicationParmeters(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetJwtIssuer()
        {
            return _configuration["JwtIssuer"];
        }

        public string GetJwtAudience()
        {
            return _configuration["JwtAudience"];
        }

        public string GetMetadataAddress()
        {
            return _configuration["MetadataAddress"];
        }

        public string GetSentimentEndpoint()
        {
            return _configuration["SentimentEndpoint"];
        }

        public string GetCustomBearerTokenSigningKey()
        {
            return _configuration["CustomBearerTokenSigningKey"];
        }

        public string GetTenantClaimKey()
        {
            return _configuration["TenantClaimKey"];
        }

        public Dictionary<string, string> GetTenants()
        {
            string tenants = _configuration["Tenants"];
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(tenants);
        }
    }
}

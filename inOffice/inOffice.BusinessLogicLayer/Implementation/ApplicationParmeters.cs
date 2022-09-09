using inOffice.BusinessLogicLayer.Interface;
using Microsoft.Extensions.Configuration;

namespace inOffice.BusinessLogicLayer.Implementation
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
            return _configuration["JwtInfo.Issuer"]; 
        }

        public string GetJwtAudience()
        {
            return _configuration["JwtInfo.Audience"];
        }

        public string GetSettingsMetadataAddress()
        {
            return _configuration["Settings.MetadataAddress"];
        }

        public string GetSettingsSentimentEndpoint()
        {
            return _configuration["Settings.SentimentEndpoint"];
        }

        public string GetSettingsUseCustomBearerToken()
        {
            return _configuration["Settings.UseCustomBearerToken"];
        }

        public string GetSettingsCustomBearerTokenSigningKey()
        {
            return _configuration["Settings.CustomBearerTokenSigningKey"];
        }
    }
}

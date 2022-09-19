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
    }
}

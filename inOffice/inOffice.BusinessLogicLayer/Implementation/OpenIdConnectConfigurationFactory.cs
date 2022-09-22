using inOffice.BusinessLogicLayer.Interface;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class OpenIdConnectConfigurationFactory : IOpenIdConnectConfigurationFactory
    {
        private readonly IApplicationParmeters _applicationParmeters;
        public OpenIdConnectConfigurationFactory(IApplicationParmeters applicationParmeters)
        {
            _applicationParmeters = applicationParmeters;
        }

        public OpenIdConnectConfiguration Create()
        {
            string metadataAddress = _applicationParmeters.GetMetadataAddress();

            if (!string.IsNullOrEmpty(metadataAddress))
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                        new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever());
                return configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
            }

            return null;
        }
    }
}

using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IOpenIdConnectConfigurationFactory
    {
        OpenIdConnectConfiguration Create();
    }
}

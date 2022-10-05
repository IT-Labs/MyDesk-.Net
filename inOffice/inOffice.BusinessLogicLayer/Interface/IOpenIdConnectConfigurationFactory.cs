using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Tokens;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IOpenIdConfigurationKeysFactory
    {
        IEnumerable<SecurityKey> GetKeys(AuthTypes authType);
    }
}

using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Tokens;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IOpenIdConfigurationKeysFactory
    {
        IEnumerable<SecurityKey> GetKeys(AuthTypes authType);
    }
}

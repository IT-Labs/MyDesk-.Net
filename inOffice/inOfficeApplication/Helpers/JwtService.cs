using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inOfficeApplication.Helpers
{
    public class JwtService
    {
        private string secureKey = "this is a very secure key";
        

        public  string Generate(int id, string role)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));

            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);
            
            var payload = new JwtPayload(id.ToString(), null,null, null, DateTime.Now.AddHours(24));
            payload.AddClaim(new Claim("role",role));


            var securityToken = new JwtSecurityToken(header, payload);


            return new JwtSecurityTokenHandler().WriteToken(securityToken);

        }
         
        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secureKey);

            //validating token
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatenToken);

            return (JwtSecurityToken) validatenToken;
        }

    }
}

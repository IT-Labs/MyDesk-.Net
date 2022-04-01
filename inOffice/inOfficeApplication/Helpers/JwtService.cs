using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inOfficeApplication.Helpers
{
    public class JwtService
    {
        private string secureKey = "this is a very secure key";

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAdminRepository _adminRepository;

        public JwtService(IEmployeeRepository employeeRepository, IAdminRepository adminRepository)
        {
            _employeeRepository = employeeRepository;
            _adminRepository = adminRepository;
        }


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

        public Employee EmployeeRoleVerification(string authHeader)
        {
            var jwt = authHeader.Substring(7);
            var token = Verify(jwt);
            
            int employeeId = int.Parse(token.Issuer);
            Employee employee = _employeeRepository.GetById(employeeId);
            
            return employee;
        }

        public object AdminRoleVerification(string authHeader)
        {
            var jwt = authHeader.Substring(7);
            var token = Verify(jwt);
            
            int adminId = int.Parse(token.Issuer);
            var admin = _adminRepository.GetById(adminId);

            return admin;
        }

    }
}

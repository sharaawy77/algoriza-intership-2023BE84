using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Vezeeta_WebSite.Utilities
{
    public  class JWT
    {
        private readonly IConfiguration configuration;

        public JWT(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public  Tuple<string,DateTime> CreateJWT(string userId,string username,List<string> Roles)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["JWT:issuer"],
                audience: configuration["JWT:audience"],
                claims:claims,
                expires:DateTime.Now.AddHours(2),
                signingCredentials:credentials
                );
            var Response=new Tuple<string, DateTime>(new JwtSecurityTokenHandler().WriteToken(token),token.ValidTo);

            return Response;
        }
    }
}

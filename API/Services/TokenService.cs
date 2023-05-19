using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entidades;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenServices
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["KeyJWT"]));
        }
        public string CrearToken(Usuario Usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, Usuario.Nombre),
            };

            //credenciales
            var credenciales = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //descriptor del token
            var descriptorToken = new SecurityTokenDescriptor{
                Expires = DateTime.Now.AddDays(7),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credenciales
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(descriptorToken);

            return tokenHandler.WriteToken(token);                                                       
        }
    }
}
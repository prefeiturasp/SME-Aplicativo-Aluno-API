using Microsoft.IdentityModel.Tokens;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SME.AE.Api.Services
{
    public class 
        TokenService
    {
        public static string GerarToken(UsuarioDto usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configs.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Cpf),
                    new Claim(ClaimTypes.Role, usuario.Grupo)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

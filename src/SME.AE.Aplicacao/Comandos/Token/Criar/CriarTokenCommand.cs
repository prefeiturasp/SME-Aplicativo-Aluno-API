using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using SME.AE.Comum;

namespace SME.AE.Aplicacao.Comandos.Token.Criar
{
    public class CriarTokenCommand : IRequest<string>
    {
        public string Usuario { get; set; }
        
        public CriarTokenCommand(string usuario)
        {
            Usuario = usuario;
        }
    }
    
    public class CriarTokenCommandHandler : IRequestHandler<CriarTokenCommand, string>
    {
        private readonly string JwtTokenSecret;
        
        public CriarTokenCommandHandler(VariaveisGlobaisOptions variaveisGlobais)
        {
            JwtTokenSecret = variaveisGlobais.SME_AE_JWT_TOKEN_SECRET;
        }

        public async Task<string> Handle(CriarTokenCommand request, CancellationToken cancellationToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(JwtTokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Usuario) 
                }),
                Issuer = "self",
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }
    }
}
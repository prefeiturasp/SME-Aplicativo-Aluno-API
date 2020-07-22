using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

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
        private readonly IUsuarioRepository _repository;


        private IMemoryCache _cache;
        public CriarTokenCommandHandler(IUsuarioRepository repository, IMemoryCache memoryCache)
        {
            _repository = repository;
            _cache = memoryCache;
        }

        public async Task<string> Handle(CriarTokenCommand request, CancellationToken cancellationToken)
        {
            RevogarToken(request, _cache);
            var token  = CriarToken(request );
            _cache.Set(request.Usuario, token, TimeSpan.FromHours(2));
            return token;
        }
        
        private static void RevogarToken(CriarTokenCommand request, IMemoryCache cache)
        {
           var token =  cache.Get(request.Usuario );
           if(token != null)
            {
                cache.Remove(request.Usuario);
            }

        }
        private static string CriarToken(CriarTokenCommand request)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(VariaveisAmbiente.JwtTokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Usuario)
                }),
                Issuer = "self",
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

         
           SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
           
            return tokenHandler.WriteToken(token);
        }
    }
}
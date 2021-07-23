using MediatR;
using Microsoft.IdentityModel.Tokens;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterSgpTokenQueryHandler : IRequestHandler<ObterSgpTokenQuery, string>
    {
        private readonly SgpJwtOptions configuracaoJwtOptions;

        public ObterSgpTokenQueryHandler(SgpJwtOptions configuracaoJwtOptions)
        {
            this.configuracaoJwtOptions = configuracaoJwtOptions ?? throw new ArgumentNullException(nameof(configuracaoJwtOptions));
        }
        public Task<string> Handle(ObterSgpTokenQuery request, CancellationToken cancellationToken)
        {
            IList<Claim> claims = new List<Claim>();

            var now = DateTime.Now;
            var token = new JwtSecurityToken(
                issuer: configuracaoJwtOptions.Issuer,
                audience: configuracaoJwtOptions.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(int.Parse(configuracaoJwtOptions.ExpiresInMinutes)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuracaoJwtOptions.IssuerSigningKey)),
                        SecurityAlgorithms.HmacSha256)
                );

            return Task.FromResult(new JwtSecurityTokenHandler()
                      .WriteToken(token));
            
        }
    }
}

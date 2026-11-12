using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Comum;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.Token
{
    public class CriarTokenCommandTeste
    {
        [Fact]
        public void Command_DeveArmazenarUsuario()
        {
            var command = new CriarTokenCommand("usuario-teste");

            Assert.Equal("usuario-teste", command.Usuario);
        }

        [Fact]
        public async Task Handle_DeveCriarTokenComUsuarioIssuerEValidade()
        {
            var handler = new CriarTokenCommandHandler(new VariaveisGlobaisOptions
            {
                SME_AE_JWT_TOKEN_SECRET = "segredo-do-token-com-mais-de-32-caracteres"
            });

            var token = await handler.Handle(new CriarTokenCommand("usuario-teste"), CancellationToken.None);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.NotEmpty(token);
            Assert.Equal("self", jwt.Issuer);
            Assert.Equal("usuario-teste", jwt.Claims.Single(x => x.Value == "usuario-teste").Value);
            Assert.True(jwt.ValidTo > DateTime.UtcNow.AddDays(29));
        }
    }
}

using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AdicionarSenhaHistoricoCommandTeste : BaseTeste
    {
        private readonly AdicionarSenhaHistoricoCommandHandler handler;
        private readonly Mock<IUsuarioSenhaHistoricoCoreSSORepositorio> repositorio;

        public AdicionarSenhaHistoricoCommandTeste()
        {
            repositorio = new Mock<IUsuarioSenhaHistoricoCoreSSORepositorio>();
            handler = new AdicionarSenhaHistoricoCommandHandler(repositorio.Object);
        }

        [Theory(DisplayName = "Deve inserir senha historico")]
        [InlineData("5b19cabf-bac5-4cd0-a249-add9794a4c9a", "Teste12345#")]
        [InlineData("5b19cabf-bac5-4cd0-a249-add9794a4c9a", "Teste#12345")]
        public async Task Deve_Inserir_Senha_Historico(Guid usuarioId, string senha)
        {
            MocarRepositorios();

            var comando = new AdicionarSenhaHistoricoCommand(usuarioId, senha);

            await handler.Handle(comando, new CancellationToken());
        }

        [Theory(DisplayName = "Deve Validar Validator")]
        [InlineData("5b19cabf-bac5-4cd0-a249-add9794a4c9a", "Teste12345#", true)]
        [InlineData("00000000-0000-0000-0000-000000000000", "Teste12345#", false)]
        [InlineData("00000000-0000-0000-0000-000000000000", "", false)]
        [InlineData("5b19cabf-bac5-4cd0-a249-add9794a4c9a", "", false)]
        public void ValidarValidator(Guid usuarioId, string senha, bool resultadoEsperado)
        {
            var comando = new AdicionarSenhaHistoricoCommand(usuarioId, senha);

            var validator = new AdicionarSenhaHistoricoCommandValidator();

            var resultado = validator.TestValidate(comando);

            Assert.Equal(resultadoEsperado, resultado.IsValid);
        }

        private void MocarRepositorios()
        {
            repositorio.Setup(x => x.AdicionarSenhaHistorico(It.IsAny<UsuarioSenhaHistoricoCoreSSO>()));
        }
    }
}

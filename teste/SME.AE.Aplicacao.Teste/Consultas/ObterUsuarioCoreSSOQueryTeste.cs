using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class ObterUsuarioCoreSSOQueryTeste : BaseTeste
    {
        private readonly ObterUsuarioCoreSSOQueryHandler obterUsuarioCoreSSOQueryHandler;
        private readonly Mock<IUsuarioCoreSSORepositorio> usuarioCoreSSORepositorio;
        private readonly Mock<IUsuarioGrupoRepositorio> usuarioGrupoRepositorio;

        public ObterUsuarioCoreSSOQueryTeste()
        {
            usuarioCoreSSORepositorio = new Mock<IUsuarioCoreSSORepositorio>();
            usuarioGrupoRepositorio = new Mock<IUsuarioGrupoRepositorio>();
            obterUsuarioCoreSSOQueryHandler = new ObterUsuarioCoreSSOQueryHandler(usuarioCoreSSORepositorio.Object, usuarioGrupoRepositorio.Object);
        }

        [Theory(DisplayName = "Deve obter o usuário por Id")]
        [InlineData("6c48bae4-4740-47e1-a0bd-66a444b39d9e")]
        [InlineData("d8f99786-7219-4b6d-9977-df1787702401")]
        [InlineData("a6ad7750-ec45-4c63-ae75-e23be9115384")]
        [InlineData("a9402ba9-b5e6-452c-b5d6-d0e4fb179349")]
        public async Task Deve_Obter_Usuario_Id(string usuidString)
        {
            obterUsuarioCoreSSOQuery.Cpf = string.Empty;
            obterUsuarioCoreSSOQuery.UsuarioId = Guid.Parse(usuidString);

            InstanciarSetup(usuId: obterUsuarioCoreSSOQuery.UsuarioId);

            var result = await obterUsuarioCoreSSOQueryHandler.Handle(obterUsuarioCoreSSOQuery, new CancellationToken());

            Assert.NotNull(result);
            Assert.Equal(obterUsuarioCoreSSOQuery.UsuarioId, result.UsuId);
        }

        // CPF gerado aleatoriamente para fins de testes
        [Theory(DisplayName = "Deve obter usuário por CPF")]
        [InlineData("77500711050")]
        [InlineData("57595471001")]
        [InlineData("78829346071")]
        [InlineData("17032464033")]
        public async Task Deve_Obter_Usuario_Cpf(string cpf)
        {
            obterUsuarioCoreSSOQuery.Cpf = cpf;
            obterUsuarioCoreSSOQuery.UsuarioId = Guid.Empty;

            InstanciarSetup(cpf: obterUsuarioCoreSSOQuery.Cpf);

            var result = await obterUsuarioCoreSSOQueryHandler.Handle(obterUsuarioCoreSSOQuery, new CancellationToken());

            Assert.NotNull(result);
            Assert.Equal(obterUsuarioCoreSSOQuery.Cpf, result.Cpf);
        }

        [Fact(DisplayName = "Deve retornar usuário nulo")]
        public async Task Deve_Retornar_Null()
        {
            var result = await obterUsuarioCoreSSOQueryHandler.Handle(obterUsuarioCoreSSOQuery, new CancellationToken());

            Assert.Null(result);
        }

        [Theory(DisplayName = "Deve validar Query Validator")]
        [InlineData("77500711050", "6c48bae4-4740-47e1-a0bd-66a444b39d9e", true)]
        [InlineData("", "", false)]
        [InlineData("78829346071", "a6ad7750-ec45-4c63-ae75-e23be9115384", true)]
        [InlineData("78829346071", "", true)]
        [InlineData("", "a9402ba9-b5e6-452c-b5d6-d0e4fb179349", true)]
        public void Deve_Validar_Query_Validator(string Cpf, string usuarioId, bool resultadoValido)
        {
            var usuIdres = Guid.TryParse(usuarioId, out Guid usuId);

            obterUsuarioCoreSSOQuery = new ObterUsuarioCoreSSOQuery(usuarioId: usuIdres ? usuId : Guid.Empty)
            {
                Cpf = Cpf
            };

            var validator = new ObterUsuarioCoreSSOQueryValidator();

            var testResult = validator.TestValidate(obterUsuarioCoreSSOQuery);

            Assert.Equal(resultadoValido, testResult.IsValid);
        }


        // CPF gerado aleatoriamente para fins de testes
        private void InstanciarSetup(string cpf = "77500711050", Guid usuId = default)
        {
            usuarioCoreSSORepositorio.Setup(x => x.ObterPorCPF(It.IsAny<string>())).ReturnsAsync(new RetornoUsuarioCoreSSO { Cpf = cpf });
            usuarioCoreSSORepositorio.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(new RetornoUsuarioCoreSSO { UsuId = usuId });
            usuarioGrupoRepositorio.Setup(x => x.ObterPorUsuarioId(It.IsAny<Guid>())).ReturnsAsync(new List<UsuarioGrupoCoreSSO>());
        }

        private ObterUsuarioCoreSSOQuery obterUsuarioCoreSSOQuery { get; set; } = new ObterUsuarioCoreSSOQuery(usuarioId: Guid.NewGuid());
    }
}

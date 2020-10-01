using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class ObterUsuarioQueryTeste : BaseTeste
    {
        private readonly Mock<IUsuarioRepository> usuarioRepository;
        private readonly ObterUsuarioQueryHandler obterUsuarioQueryHandler;
        private readonly ObterUsuarioQueryValidator validator;

        public ObterUsuarioQueryTeste()
        {
            usuarioRepository = new Mock<IUsuarioRepository>();
            obterUsuarioQueryHandler = new ObterUsuarioQueryHandler(usuarioRepository.Object);
            validator = new ObterUsuarioQueryValidator();
        }

        [Fact(DisplayName = "Deve acusar Usuário não encontrado")]
        public async Task Deve_Acusar_Usuario_Nao_Encontrado()
        {
            await Assert.ThrowsAsync<NegocioException>(async () => await obterUsuarioQueryHandler.Handle(new ObterUsuarioQuery(), new CancellationToken()));
        }

        [Fact(DisplayName = "Deve obter usuário")]
        public async Task Deve_Obter_Usuario()
        {
            InstanciarSetups();

            await obterUsuarioQueryHandler.Handle(obterUsuarioQuery, new CancellationToken());
        }

        [Fact(DisplayName = "Deve validar Id")]
        public void Deve_Validar_Id()
        {
            obterUsuarioQuery.Cpf = string.Empty;

            var result = ValidarObjeto();

            result.ShouldNotHaveAnyValidationErrors();

            obterUsuarioQuery.Id = 0;

            result = ValidarObjeto();

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact(DisplayName = "Deve validar CPF")]
        public void Deve_Validar_CPF()
        {
            obterUsuarioQuery.Cpf = "";
            obterUsuarioQuery.Id = 0;

            var result = ValidarObjeto();

            result.ShouldHaveValidationErrorFor(x => x.Cpf);

            obterUsuarioQuery.Cpf = "84989162080";

            result = ValidarObjeto();

            result.ShouldNotHaveAnyValidationErrors();
        }

        private TestValidationResult<ObterUsuarioQuery, ObterUsuarioQuery> ValidarObjeto()
        {
            var validator = new ObterUsuarioQueryValidator();

            return validator.TestValidate(obterUsuarioQuery);
        }

        private void InstanciarSetups()
        {
            usuarioRepository.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf(It.IsAny<string>())).ReturnsAsync(new Usuario());

            usuarioRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<long>())).ReturnsAsync(new Usuario());
        }

        //CPF gerado aleatoriamente por uma ferramenta que gera CPF
        public ObterUsuarioQuery obterUsuarioQuery { get; set; } = new ObterUsuarioQuery
        {
            Cpf = "84989162080",
            Id = 1
        };
    }
}

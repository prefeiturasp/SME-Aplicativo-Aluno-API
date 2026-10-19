using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class AtualizarDadosUsuarioProdamUseCaseTeste : BaseTeste
    {
        private const int IdUsuario = 1;
        private const string CpfUsuario = "12345678900";
        private const string CpfResponsavelEol = "98765432100";
        private const string CodigoAluno = "1234567";
        private const string OutroCodigoAluno = "7654321";

        private readonly IAtualizarDadosUsuarioProdamUseCase atualizarDadosUsuarioProdamUseCase;
        private readonly List<EnviarAtualizacaoCadastralProdamCommand> atualizacoesEnviadas;

        public AtualizarDadosUsuarioProdamUseCaseTeste()
        {
            atualizarDadosUsuarioProdamUseCase = new AtualizarDadosUsuarioProdamUseCase(mediator.Object);
            atualizacoesEnviadas = new List<EnviarAtualizacaoCadastralProdamCommand>();

            mediator.Setup(m => m.Send(It.IsAny<EnviarAtualizacaoCadastralProdamCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<bool>, CancellationToken>((request, token) => atualizacoesEnviadas.Add((EnviarAtualizacaoCadastralProdamCommand)request))
                    .ReturnsAsync(true);
        }

        [Fact(DisplayName = "Deve enviar uma atualização cadastral à Prodam para cada aluno vinculado ao responsável")]
        public async Task Deve_Enviar_Uma_Atualizacao_Cadastral_A_Prodam_Para_Cada_Aluno_Vinculado_Ao_Responsavel()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(CriarResponsavel(CodigoAluno), CriarResponsavel(OutroCodigoAluno));

            var resultado = await atualizarDadosUsuarioProdamUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.True(resultado);
            Assert.Equal(new[] { CodigoAluno, OutroCodigoAluno }, atualizacoesEnviadas.Select(a => a.ResponsavelDto.CodigoAluno));
            mediator.Verify(m => m.Send(It.Is<ObterUsuarioQuery>(q => q.Id == IdUsuario && q.Cpf == null), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.Is<ObterDadosResponsavelQuery>(q => q.CpfResponsavel == CpfUsuario), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve aplicar os dados informados e preservar os demais dados do EOL")]
        public async Task Deve_Aplicar_Os_Dados_Informados_E_Preservar_Os_Demais_Dados_Do_EOL()
        {
            var dadosAtualizacao = CriarDto();
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(CriarResponsavel(CodigoAluno));

            await atualizarDadosUsuarioProdamUseCase.Executar(CriarMensagem(dadosAtualizacao));

            var responsavelEnviado = Assert.Single(atualizacoesEnviadas).ResponsavelDto;
            Assert.Equal("19850510", responsavelEnviado.DataNascimentoMae);
            Assert.Equal(dadosAtualizacao.NomeMae, responsavelEnviado.NomeMae);
            Assert.Equal(dadosAtualizacao.Email, responsavelEnviado.Email);
            Assert.Equal("11", responsavelEnviado.DDDCelular);
            Assert.Equal("987654321", responsavelEnviado.NumeroCelular);
            Assert.Equal("Maria", responsavelEnviado.Nome);

            Assert.Equal(CodigoAluno, responsavelEnviado.CodigoAluno);
            Assert.Equal(CpfResponsavelEol, responsavelEnviado.CPF);
            Assert.Equal("S", responsavelEnviado.CPFConfere);
            Assert.Equal("123456789", responsavelEnviado.NumeroRG);
            Assert.Equal("0", responsavelEnviado.DigitoRG);
            Assert.Equal("SP", responsavelEnviado.UfRG);
            Assert.Equal("1", responsavelEnviado.TipoTurnoCelular);
            Assert.Equal("11", responsavelEnviado.DDDTelefoneFixo);
            Assert.Equal("34567890", responsavelEnviado.NumeroTelefoneFixo);
            Assert.Equal("2", responsavelEnviado.TipoTurnoTelefoneFixo);
            Assert.Equal("11", responsavelEnviado.DDDTelefoneComercial);
            Assert.Equal("23456789", responsavelEnviado.NumeroTelefoneComercial);
            Assert.Equal("3", responsavelEnviado.TipoTurnoTelefoneComercial);
            Assert.Equal("S", responsavelEnviado.AutorizaEnvioSMS);
            Assert.Equal(((int)TipoResponsavelEnum.Responsavel_Legal).ToString(), responsavelEnviado.TipoPessoa);
            Assert.Equal("webResp", responsavelEnviado.Usuario);
            Assert.Equal("resp", responsavelEnviado.Senha);
        }

        [Theory(DisplayName = "Deve definir o turno do telefone fixo")]
        [InlineData("2", "11", "34567890", "2")]
        [InlineData(null, "11", "34567890", "1")]
        [InlineData("", "11", null, "")]
        [InlineData(null, null, "34567890", "")]
        public async Task Deve_Definir_O_Turno_Do_Telefone_Fixo(string turnoAtual, string dddFixo, string numeroFixo, string turnoEsperado)
        {
            var responsavel = CriarResponsavel(CodigoAluno);
            responsavel.TipoTurnoTelefoneFixo = turnoAtual;
            responsavel.DDDTelefoneFixo = dddFixo;
            responsavel.NumeroTelefoneFixo = numeroFixo;
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(responsavel);

            await atualizarDadosUsuarioProdamUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.Equal(turnoEsperado, Assert.Single(atualizacoesEnviadas).ResponsavelDto.TipoTurnoTelefoneFixo);
        }

        [Fact(DisplayName = "Deve retornar false quando o usuário não existir na aplicação")]
        public async Task Deve_Retornar_False_Quando_O_Usuario_Nao_Existir_Na_Aplicacao()
        {
            MockarUsuario(null);

            var resultado = await atualizarDadosUsuarioProdamUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.False(resultado);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Empty(atualizacoesEnviadas);
        }

        [Theory(DisplayName = "Deve retornar false quando não houver responsáveis no EOL")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Deve_Retornar_False_Quando_Nao_Houver_Responsaveis_No_EOL(bool retornoNulo)
        {
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(retornoNulo ? null : Array.Empty<DadosResponsavelAluno>());

            var resultado = await atualizarDadosUsuarioProdamUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.False(resultado);
            Assert.Empty(atualizacoesEnviadas);
        }

        private void MockarUsuario(Dominio.Entidades.Usuario usuario)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);
        }

        private void MockarResponsaveisEol(params DadosResponsavelAluno[] responsaveis)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(responsaveis);
        }

        private static MensagemRabbit CriarMensagem(AtualizarDadosUsuarioDto dadosAtualizacao)
        {
            return new MensagemRabbit(JsonConvert.SerializeObject(dadosAtualizacao), Guid.NewGuid());
        }

        private static AtualizarDadosUsuarioDto CriarDto()
        {
            return new AtualizarDadosUsuarioDto
            {
                Id = IdUsuario,
                Email = "responsavel@teste.com",
                NomeMae = "Maria da Silva",
                Celular = "(11) 98765-4321",
                DataNascimentoResponsavel = new DateTime(1985, 5, 10)
            };
        }

        private static Dominio.Entidades.Usuario CriarUsuario()
        {
            return new Dominio.Entidades.Usuario
            {
                Id = IdUsuario,
                Cpf = CpfUsuario
            };
        }

        private static DadosResponsavelAluno CriarResponsavel(string codigoAluno)
        {
            return new DadosResponsavelAluno
            {
                CodigoAluno = codigoAluno,
                Cpf = CpfResponsavelEol,
                Nome = "  Maria  ",
                TipoResponsavel = TipoResponsavelEnum.Responsavel_Legal,
                Email = "email.antigo@teste.com",
                NomeMae = "Nome da Mãe Antigo",
                DataNascimentoMae = new DateTime(1960, 1, 1),
                DDDCelular = "21",
                NumeroCelular = "912345678",
                TipoTurnoCelular = "1",
                NumeroRG = "123456789",
                DigitoRG = "0",
                UfRG = "SP",
                CPFConfere = "S",
                DDDTelefoneFixo = "11",
                NumeroTelefoneFixo = "34567890",
                TipoTurnoTelefoneFixo = "2",
                DDDTelefoneComercial = "11",
                NumeroTelefoneComercial = "23456789",
                TipoTurnoTelefoneComercial = "3",
                AutorizaEnvioSMS = "S"
            };
        }
    }
}

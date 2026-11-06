using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Notificacao
{
    public class AtualizarNotificacaoUseCaseTeste : BaseTeste
    {
        private readonly AtualizarNotificacaoUseCase atualizarNotificacaoUseCase;

        public AtualizarNotificacaoUseCaseTeste()
        {
            atualizarNotificacaoUseCase = new AtualizarNotificacaoUseCase(mediator.Object, mapper.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando Mediator for nulo no construtor")]
        public void DeveLancarArgumentNullException_QuandoMediatorNulo()
        {
            Assert.Throws<ArgumentNullException>(() => new AtualizarNotificacaoUseCase(null, mapper.Object));
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao SME com sucesso")]
        public async Task DeveAtualizarNotificacaoSME()
        {
            var dto = NotificacaoSME;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao DRE com sucesso")]
        public async Task DeveAtualizarNotificacaoDRE()
        {
            var dto = NotificacaoDRE;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao UE com sucesso")]
        public async Task DeveAtualizarNotificacaoUE()
        {
            var dto = NotificacaoUE;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao UEMOD com sucesso")]
        public async Task DeveAtualizarNotificacaoUEMOD()
        {
            var dto = NotificacaoUEMOD;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao TURMA com sucesso")]
        public async Task DeveAtualizarNotificacaoTURMA()
        {
            var dto = NotificacaoTurma;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve Atualizar Notificacao ALUNO com sucesso")]
        public async Task DeveAtualizarNotificacaoALUNO()
        {
            var dto = NotificacaoAluno;
            InstanciarSetupSucesso(dto);

            var resultado = await atualizarNotificacaoUseCase.Executar(dto);

            ValidarRetornoEChamadaMediator(dto, resultado);
        }

        [Fact(DisplayName = "Deve lançar NegocioException quando Mediator retornar nulo")]
        public async Task DeveLancarNegocioException_QuandoMediatorRetornarNulo()
        {
            var dto = NotificacaoSME;
            MediatorSetup<AtualizarNotificacaoCommand, AtualizacaoNotificacaoResposta>(null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() => atualizarNotificacaoUseCase.Executar(dto));

            Assert.Equal("Não foi possível atualizar o comunicado na base do Escola Aqui!", excecao.Message);
            mediator.Verify(x => x.Send(It.IsAny<AtualizarNotificacaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar NullReferenceException quando NotificacaoSgpDto for nula")]
        public async Task DeveLancarExcecao_QuandoParametroNotificacaoForNulo()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => atualizarNotificacaoUseCase.Executar(null));
        }

        private void InstanciarSetupSucesso(NotificacaoSgpDto dto)
        {
            var respostaEsperada = new AtualizacaoNotificacaoResposta
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Mensagem = dto.Mensagem,
                DataExpiracao = dto.DataExpiracao
            };

            MediatorSetup<AtualizarNotificacaoCommand, AtualizacaoNotificacaoResposta>(respostaEsperada);
        }

        private void ValidarRetornoEChamadaMediator(NotificacaoSgpDto dto, AtualizacaoNotificacaoResposta resultado)
        {
            Assert.NotNull(resultado);
            Assert.Equal(dto.Id, resultado.Id);
            Assert.Equal(dto.Titulo, resultado.Titulo);
            Assert.Equal(dto.Mensagem, resultado.Mensagem);
            Assert.Equal(dto.DataExpiracao, resultado.DataExpiracao);

            mediator.Verify(x => x.Send(
                It.Is<AtualizarNotificacaoCommand>(cmd =>
                    cmd.Id == dto.Id &&
                    cmd.Titulo == dto.Titulo &&
                    cmd.Mensagem == dto.Mensagem &&
                    cmd.DataExpiracao == dto.DataExpiracao &&
                    cmd.AlteradoPor == dto.AlteradoPor),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        private static NotificacaoSgpDto NotificacaoSME => new()
        {
            Id = 1001,
            Titulo = "Teste SME",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.SME,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem SME",
            ModalidadesId = new[] { "1", "2", "3", "4", "5", "6" },
            DataExpiracao = DateTime.Now.AddDays(7),
            CriadoPor = "Sistema",
            AlteradoPor = "UsuarioSME"
        };

        private static NotificacaoSgpDto NotificacaoDRE => new()
        {
            Id = 1002,
            Titulo = "Teste DRE",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.DRE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem DRE",
            CodigoDre = "000000",
            DataExpiracao = DateTime.Now.AddDays(7),
            AlteradoPor = "UsuarioDRE"
        };

        private static NotificacaoSgpDto NotificacaoUE => new()
        {
            Id = 1003,
            Titulo = "Teste UE",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem UE",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            DataExpiracao = DateTime.Now.AddDays(7),
            AlteradoPor = "UsuarioUE"
        };

        private static NotificacaoSgpDto NotificacaoUEMOD => new()
        {
            Id = 1004,
            Titulo = "Teste UEMOD",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UEMOD,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem UEMOD",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            ModalidadesId = new[] { "1", "2", "3", "4", "5", "6" },
            DataExpiracao = DateTime.Now.AddDays(7),
            AlteradoPor = "UsuarioUEMOD"
        };

        private static NotificacaoSgpDto NotificacaoTurma => new()
        {
            Id = 1005,
            Titulo = "Teste Turma",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.TURMA,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem Turma",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            ModalidadesId = new[] { "1", "2", "3", "4", "5", "6" },
            Turmas = new List<string> { "0000000" },
            DataExpiracao = DateTime.Now.AddDays(7),
            AlteradoPor = "UsuarioTurma"
        };

        private static NotificacaoSgpDto NotificacaoAluno => new()
        {
            Id = 1006,
            Titulo = "Teste Aluno",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.ALUNO,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem Aluno",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            ModalidadesId = new[] { "1", "2", "3", "4", "5", "6" },
            Turmas = ["0000000"],
            Alunos = ["0000000"],
            DataExpiracao = DateTime.Now.AddDays(7),
            AlteradoPor = "UsuarioAluno"
        };
    }
}
using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntidadeNotificacao = SME.AE.Dominio.Entidades.Notificacao;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Notificacao
{
    public class CriarNotificacaoUseCaseTeste : BaseTeste
    {
        private readonly ICriarNotificacaoUseCase criarNotificacaoUseCase;

        public CriarNotificacaoUseCaseTeste()
        {
            criarNotificacaoUseCase = new CriarNotificacaoUseCase(mediator.Object, mapper.Object);
        }

        private void ConfigurarMocks(NotificacaoSgpDto dto)
        {
            var entidadeEsperada = new EntidadeNotificacao
            {
                Id = 1,
                Titulo = dto.Titulo,
                AnoLetivo = dto.AnoLetivo,
                TipoComunicado = (Dominio.Comum.Enumeradores.TipoComunicado)dto.TipoComunicado,
                DataEnvio = dto.DataEnvio,
                Mensagem = dto.Mensagem,
                CodigoDre = dto.CodigoDre,
                CodigoUe = dto.CodigoUe,
                Modalidades = dto.Modalidades,
                Turmas = dto.Turmas,
                Alunos = dto.Alunos,
                DataExpiracao = dto.DataExpiracao
            };
            entidadeEsperada.InserirCategoria();

            mediator.Setup(x => x.Send(It.IsAny<IRequest<EntidadeNotificacao>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(entidadeEsperada);

            mediator.Setup(x => x.Send(It.IsAny<EnviarNotificacaoPorGrupoCommand>(), It.IsAny<CancellationToken>()));

            mapper.Setup(x => x.Map<EntidadeNotificacao>(It.IsAny<object>()))
                  .Returns(entidadeEsperada);
            mapper.Setup(x => x.Map<NotificacaoSgpDto, EntidadeNotificacao>(It.IsAny<NotificacaoSgpDto>()))
                  .Returns(entidadeEsperada);
        }

        [Fact(DisplayName = "Deve criar as notificações SME")]
        public async Task ValidarEnvioNotificacaoSME()
        {
            var dto = NotificacaoSME;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Titulo, result.Titulo);
            Assert.Equal("SME", result.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve criar as notificações DRE")]
        public async Task ValidarEnvioNotificacaoDRE()
        {
            var dto = NotificacaoDRE;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.CodigoDre, result.CodigoDre);
            Assert.Equal("DRE", result.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve criar as notificações UE")]
        public async Task ValidarEnvioNotificacaoUE()
        {
            var dto = NotificacaoUE;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.CodigoUe, result.CodigoUe);
            Assert.Equal("UE", result.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve criar as notificações UE MOD")]
        public async Task ValidarEnvioNotificacaoUEMOD()
        {
            var dto = NotificacaoUEMOD;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Modalidades, result.Modalidades);
            Assert.Equal("UE", result.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve criar as notificações TURMA")]
        public async Task ValidarEnvioNotificacaoTURMA()
        {
            var dto = NotificacaoTurma;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Turmas);
            Assert.Equal("UE", result.CategoriaNotificacao);
        }

        [Fact(DisplayName = "Deve criar as notificações ALUNO")]
        public async Task ValidarEnvioNotificacaoALUNO()
        {
            var dto = NotificacaoAluno;
            ConfigurarMocks(dto);

            var result = await criarNotificacaoUseCase.Executar(dto);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Alunos);
            Assert.Equal("UE", result.CategoriaNotificacao);
        }

        private NotificacaoSgpDto NotificacaoSME => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.SME,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            Modalidades = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoDRE => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.DRE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoUE => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoUEMOD => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UEMOD,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoTurma => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.TURMA,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            Turmas = new List<string> { "0000000" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoAluno => new()
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.ALUNO,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            Turmas = new List<string> { "0000000" },
            Alunos = new List<string> { "0000000" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };
    }
}
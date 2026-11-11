using MediatR;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.Recomendacoes;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterRecomendacaoAlunoUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterRecomendacaoAlunoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_QuandoHouverRecomendacao_DeveMapearPrimeiroRegistro()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterRecomendacoesPorAlunosTurmasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([

                    new RecomendacaoConselhoClasseAluno { AlunoCodigo = "A1", TurmaCodigo = "T1", AnotacoesPedagogicas = "anotação", RecomendacoesAluno = "aluno", RecomendacoesFamilia = "família" },
                    new RecomendacaoConselhoClasseAluno { AlunoCodigo = "A2" }
                ]);
            var useCase = new ObterRecomendacaoAlunoUseCase(mediator.Object);
            var filtro = new FiltroRecomendacaoAlunoDto { CodigoAluno = "A1", CodigoTurma = "T1", AnoLetivo = 2025, Modalidade = (int)ModalidadeDeEnsino.Fundamental, Semestre = 1 };

            var resultado = await useCase.Executar(filtro);

            Assert.Equal("A1", resultado.AlunoCodigo);
            Assert.Equal("T1", resultado.TurmaCodigo);
            Assert.Equal("anotação", resultado.AnotacoesPedagogicas);
            Assert.Equal("aluno", resultado.RecomendacoesAluno);
            Assert.Equal("família", resultado.RecomendacoesFamilia);
            mediator.Verify(x => x.Send(It.Is<ObterRecomendacoesPorAlunosTurmasQuery>(q => q.CodigoAluno == "A1" && q.CodigoTurma == "T1" && q.AnoLetivo == 2025 && q.Modalidade == ModalidadeDeEnsino.Fundamental && q.Semestre == 1), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoConsultaVaziaOuNula_DeveRetornarAlerta()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterRecomendacoesPorAlunosTurmasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<RecomendacaoConselhoClasseAluno>)null);
            var useCase = new ObterRecomendacaoAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FiltroRecomendacaoAlunoDto());

            Assert.Equal("Sem recomendações para este Aluno", resultado.MensagemAlerta);
            Assert.Null(resultado.AlunoCodigo);
        }

        [Fact]
        public async Task Executar_QuandoConsultaRetornarListaVazia_DeveRetornarAlerta()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterRecomendacoesPorAlunosTurmasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<RecomendacaoConselhoClasseAluno>());
            var useCase = new ObterRecomendacaoAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FiltroRecomendacaoAlunoDto());

            Assert.Equal("Sem recomendações para este Aluno", resultado.MensagemAlerta);
        }
    }
}
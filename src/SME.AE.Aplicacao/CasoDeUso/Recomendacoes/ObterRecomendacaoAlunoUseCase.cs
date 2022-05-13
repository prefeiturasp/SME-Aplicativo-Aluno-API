using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Recomendacao;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterRecomendacoesPorAlunosTurmas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Recomendacoes
{
    public class ObterRecomendacaoAlunoUseCase : IObterRecomendacaoAlunoUseCase
    {
        private readonly IMediator mediator;
        public ObterRecomendacaoAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<RecomendacaoConselhoClasseAluno> Executar(FiltroRecomendacaoAlunoDto filtro)
        {
            var recomendacao = new RecomendacaoConselhoClasseAluno();
            var consulta = await mediator.Send(new ObterRecomendacoesPorAlunosTurmasQuery(filtro.CodigoAluno, filtro.CodigoTurma, filtro.AnoLetivo,
                                                                                        (ModalidadeDeEnsino)filtro.Modalidade, filtro.Semestre));
            if (consulta?.Count() > 0)
            {
                recomendacao = new RecomendacaoConselhoClasseAluno
                {
                    AlunoCodigo = consulta.FirstOrDefault().AlunoCodigo,
                    TurmaCodigo = consulta.FirstOrDefault().TurmaCodigo,
                    AnotacoesPedagogicas = consulta.FirstOrDefault().AnotacoesPedagogicas,
                    RecomendacoesAluno = consulta.FirstOrDefault().RecomendacoesAluno,
                    RecomendacoesFamilia = consulta.FirstOrDefault().RecomendacoesFamilia
                };

            }
            else
                recomendacao.MensagemAlerta = "Sem recomendações para este Aluno";
            return recomendacao;
        }
    }
}

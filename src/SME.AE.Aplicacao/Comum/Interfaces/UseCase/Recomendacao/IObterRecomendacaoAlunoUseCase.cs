using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Recomendacao
{
    public interface IObterRecomendacaoAlunoUseCase
    {
        Task<RecomendacaoConselhoClasseAluno> Executar(FiltroRecomendacaoAlunoDto filtro);
    }
}

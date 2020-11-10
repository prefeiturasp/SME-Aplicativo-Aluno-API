using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterNotasAlunoUseCase
    {
        Task<IEnumerable<NotaAlunoResposta>> Executar(NotaAlunoDto notaAlunoDto);
    }
}


using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IObterNotasPorBimestresUeAlunoTurmaUseCase
    {
        Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Executar(NotaConceitoPorBimestresAlunoTurmaDto notaAlunoDto);
    }
}
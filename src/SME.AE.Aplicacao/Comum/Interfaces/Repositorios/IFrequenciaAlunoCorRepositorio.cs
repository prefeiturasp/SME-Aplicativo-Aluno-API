using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IFrequenciaAlunoCorRepositorio
    {
        Task<IEnumerable<FrequenciaAlunoCor>> ObterAsync();
    }
}
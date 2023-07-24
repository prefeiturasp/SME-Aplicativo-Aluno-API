using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IAlunoRepositorio
    {
        Task<List<CpfResponsavelAlunoEol>> ObterCpfsDeResponsaveis(string codigoDre, string codigoUe);
    }
}

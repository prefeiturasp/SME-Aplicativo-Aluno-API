using SME.AE.Aplicacao.Comum.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IResponsavelEOLRepositorio
    {
        Task<IEnumerable<ResponsavelAlunoEOLDto>> ListarCpfResponsavelAlunoDaDreUeTurma();
        Task<IEnumerable<ResponsavelEOLDto>> ListarCpfResponsavelDaDreUeTurma(long dreCodigo, int anoLetivo);
        Task<ResponsavelAlunoEolResumidoDto> ObterDadosResumidosReponsavelPorCpf(string cpfResponsavel);
    }
}

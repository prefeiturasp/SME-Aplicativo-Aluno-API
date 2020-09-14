using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface ITermosDeUsoRepositorio : IBaseRepositorio<TermosDeUso>
    {
        Task<TermosDeUso> ObterUltimaVersao();
        new Task<long> SalvarAsync(TermosDeUso termo);
    }
}

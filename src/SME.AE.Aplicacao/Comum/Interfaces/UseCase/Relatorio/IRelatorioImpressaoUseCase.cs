using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IRelatorioImpressaoUseCase
    {
        Task<bool> Executar(Guid codigo);
    }
}

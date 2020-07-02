using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IRemoveNotificacaoPorIdUseCase
    {
        Task<bool> Executar(int id);
    }
}

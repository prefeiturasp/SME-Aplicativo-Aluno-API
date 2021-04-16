using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;

namespace SME.AE.Aplicacao.CasoDeUso.AgendadoWorkerService
{
    public class RemoverConexaoIdleCasoDeUso
    {
        private readonly IRemoverConexaoIdleRepository removerConexaoIdleRepository;

        public RemoverConexaoIdleCasoDeUso(IRemoverConexaoIdleRepository removerConexaoIdleRepository)
        {
            this.removerConexaoIdleRepository = removerConexaoIdleRepository ?? throw new ArgumentNullException(nameof(removerConexaoIdleRepository));
        }

        public void Executar()
        {
            removerConexaoIdleRepository.RemoverConexoesIdle();
        }
    }
}

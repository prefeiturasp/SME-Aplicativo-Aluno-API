using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IWorkerProcessoAtualizacaoRepositorio
    {
        Task IncluiOuAtualizaUltimaAtualizacao(string nomeProcesso);
    }
}

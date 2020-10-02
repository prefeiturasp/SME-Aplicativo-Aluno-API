using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IEventoRepositorio
    {
        Task<DateTime?> ObterUltimaAlteracao();
        Task Salvar(EventoDto evento);
    }
}

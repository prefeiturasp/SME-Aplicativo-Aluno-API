using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IEventoSgpRepositorio
    {
        Task<IEnumerable<EventoSgpDto>> ListaEventoPorDataAlteracao(DateTime ultimaDataAlteracao, int pagina, int quantidadeRegistrosPorPagina);
    }
}

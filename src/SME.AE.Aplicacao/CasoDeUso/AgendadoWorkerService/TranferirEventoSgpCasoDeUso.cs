using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class TranferirEventoSgpCasoDeUso
    {
        private const int QUANTIDADE_REGISTROS_POR_PAGINA_EVENTOS = 1000;

        private readonly IEventoRepositorio eventoRepositorio;
        private readonly IEventoSgpRepositorio eventoSgpRepositorio;

        public TranferirEventoSgpCasoDeUso(IEventoRepositorio eventoRepositorio, IEventoSgpRepositorio eventoSgpRepositorio)
        {
            this.eventoRepositorio = eventoRepositorio ?? throw new ArgumentNullException(nameof(eventoRepositorio));
            this.eventoSgpRepositorio = eventoSgpRepositorio ?? throw new ArgumentNullException(nameof(eventoSgpRepositorio));
        }

        public async Task ExecutarAsync()
        {
            DateTime ultimaData = await ObterUltimaDataEvento();
            var contadorPaginacao = 1;

            IEnumerable<EventoSgpDto> listaEvendosSgp = null;
            do
            {
                listaEvendosSgp = await ObterListaEventosSgp(ultimaData, contadorPaginacao);
                IEnumerable<EventoDto> listaEventos = MapearEventos(listaEvendosSgp);
                await SalvarEventos(listaEventos.Where(e => !e.excluido));
                await RemoverEventos(listaEventos.Where(e => e.excluido));
                contadorPaginacao++;
            } while (listaEvendosSgp != null && listaEvendosSgp.Any());
        }

        private async Task RemoverEventos(IEnumerable<EventoDto> listaEventos)
        {
            foreach (var evento in listaEventos)
                await eventoRepositorio.Remover(evento);
        }

        private async Task SalvarEventos(IEnumerable<EventoDto> listaEventos)
        {
            foreach (var evento in listaEventos)
                await eventoRepositorio.Salvar(evento);
        }

        private IEnumerable<EventoDto> MapearEventos(IEnumerable<EventoSgpDto> listaEventosSgp)
        {
            foreach (var eventoSgp in listaEventosSgp)
            {
                var evento = new EventoDto
                {
                    evento_id = eventoSgp.ObterEventoId(),
                    ano_letivo = eventoSgp.ano_letivo,
                    data_inicio = eventoSgp.data_inicio,
                    data_fim = eventoSgp.data_fim,
                    tipo_evento = eventoSgp.tipo_evento_id,
                    turma_id = eventoSgp.turma_id,
                    nome = eventoSgp.nome,
                    descricao = eventoSgp.descricao?.Length > 5000 ? eventoSgp.descricao?.Substring(0, 5000) : eventoSgp.descricao,
                    dre_id = eventoSgp.dre_id,
                    ue_id = eventoSgp.ue_id,
                    ultima_alteracao_sgp = eventoSgp.alterado_em,
                    modalidade = eventoSgp.tipo_evento_id == (int)TipoEvento.Avaliacao
                                    ? ObterModalidadeCalendarioDeModalidadeTurma(eventoSgp)
                                    : eventoSgp.modalidade_calendario.Value,
                    excluido = eventoSgp.excluido,
                    componente_curricular = eventoSgp.componente_curricular,
                    tipo_calendario_id = eventoSgp.tipo_calendario_id
                };
                yield return evento;
            }
        }

        private static int ObterModalidadeCalendarioDeModalidadeTurma(EventoSgpDto eventoSgp)
        {
            switch (eventoSgp.modalidade_turma.Value)
            {
                case 1:
                    return 3;
                case 3:
                    return 2;
                case 5:
                case 6:
                    return 1;
            }
            return 0;
        }

        private async Task<IEnumerable<EventoSgpDto>> ObterListaEventosSgp(DateTime ultimaDataAlteracao, int pagina)
        {
            return await eventoSgpRepositorio.ListaEventoPorDataAlteracao(ultimaDataAlteracao, pagina, QUANTIDADE_REGISTROS_POR_PAGINA_EVENTOS);
        }

        private async Task<DateTime> ObterUltimaDataEvento()
        {
            DateTime? ultimaAlteracao = await eventoRepositorio.ObterUltimaAlteracao();
            if (!ultimaAlteracao.HasValue)
                ultimaAlteracao = DateTime.MinValue;
            return ultimaAlteracao.Value;
        }
    }
}

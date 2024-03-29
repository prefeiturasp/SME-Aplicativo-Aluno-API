﻿using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    class ObterEventosAlunoPorMesQueryHandler : IRequestHandler<ObterEventosAlunoPorMesQuery, IEnumerable<EventoRespostaDto>>
    {
        private readonly IMediator mediator;
        private readonly IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio;

        public ObterEventosAlunoPorMesQueryHandler(IParametrosEscolaAquiRepositorio parametrosEscolaAquiRepositorio, IMediator mediator)
        {
            this.parametrosEscolaAquiRepositorio = parametrosEscolaAquiRepositorio ?? throw new ArgumentNullException(nameof(parametrosEscolaAquiRepositorio));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<EventoRespostaDto>> Handle(ObterEventosAlunoPorMesQuery request, CancellationToken cancellationToken)
        {
            var aluno = (await mediator.Send(new ObterDadosAlunosQuery(request.Cpf, null, null, null))).Where(a => a.CodigoEol == request.CodigoAluno).FirstOrDefault();

            var turmasModalidade = await mediator.Send(new ObterTurmasModalidadesPorCodigosQuery(new string[] { aluno.CodigoTurma.ToString() }));
            if (turmasModalidade.Any())
            {
                var modalidadeDaTurma = turmasModalidade.FirstOrDefault();
                aluno.ModalidadeCodigo = modalidadeDaTurma.ModalidadeCodigo;
                aluno.ModalidadeDescricao = modalidadeDaTurma.ModalidadeDescricao;
            }

            var modalidade = 0;

            switch (aluno.ModalidadeCodigo)
            {
                case 1:
                    modalidade = 3;
                    break;
                case 3:
                    modalidade = 2;
                    break;
                case 5:
                case 6:
                    modalidade = 1;
                    break;
                default:
                    modalidade = aluno.ModalidadeCodigo;
                    break;
            }
            var eventos = await mediator.Send(new ObterEventosPorDreUeTurmaMesQuery(aluno.CodigoDre, aluno.CodigoEscola, aluno.CodigoTurma.ToString(), modalidade, request.MesAno));

            var mesInicial = parametrosEscolaAquiRepositorio.ObterInt("MesInicioTransferenciaEventos", 3);
            var diaInicial = parametrosEscolaAquiRepositorio.ObterInt("DiaInicioTransferenciaEventos", 1);
            var dataInicial = new DateTime(DateTime.Now.Year, mesInicial, diaInicial);

            if (DateTime.Today < dataInicial)
            {
                var tiposEventosPermitidos = new int[] { (int)TipoEvento.ReuniaoResponsaveis, (int)TipoEvento.Feriado, (int)TipoEvento.Avaliacao };
                eventos = eventos.Where(e => tiposEventosPermitidos.Contains(e.tipo_evento));
            }

            var eventosResposta = eventos
                .Select(
                    e => new EventoRespostaDto
                    {
                        Nome = e.nome,
                        Descricao = e.descricao,
                        DiaSemana = e.data_inicio.ToString("ddd", new CultureInfo("pt-BR")),
                        DataInicio = e.data_inicio,
                        DataFim = e.data_fim,
                        AnoLetivo = e.ano_letivo,
                        TipoEvento = e.tipo_evento,
                        ComponenteCurricular = e.componente_curricular
                    }
                ).Distinct();
            return eventosResposta;
        }
    }
}

using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    class ObterEventosAlunoPorMesQueryHandler : IRequestHandler<ObterEventosAlunoPorMesQuery, IEnumerable<EventoRespostaDto>>
    {
        private readonly IEventoRepositorio eventoRepositorio;
        private readonly IAlunoRepositorio alunoRepositorio;

        public ObterEventosAlunoPorMesQueryHandler(IEventoRepositorio eventoRepositorio, IAlunoRepositorio alunoRepositorio)
        {
            this.eventoRepositorio = eventoRepositorio ?? throw new ArgumentNullException(nameof(eventoRepositorio));
            this.alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
        }
        public async Task<IEnumerable<EventoRespostaDto>> Handle(ObterEventosAlunoPorMesQuery request, CancellationToken cancellationToken)
        {
            var aluno = (await alunoRepositorio.ObterDadosAlunos(request.Cpf)).Where(a => a.CodigoEol == request.CodigoAluno).FirstOrDefault();

            var modalidade = 0;

            switch (aluno.CodigoModalidadeTurma)
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
                    modalidade = aluno.CodigoModalidadeTurma;
                    break;
            }
            var eventos = await eventoRepositorio.ObterPorDreUeTurmaMes(aluno.CodigoDre, aluno.CodigoEscola, aluno.CodigoTurma.ToString(), modalidade, request.MesAno);
            var eventosResposta = eventos.Select(
                    e => new EventoRespostaDto
                    {
                        Nome = e.nome,
                        Descricao = e.descricao,
                        DataInicio = e.data_inicio,
                        DataFim = e.data_fim,
                        AnoLetivo = e.ano_letivo,
                        TipoEvento = e.tipo_evento
                    }
                ).Distinct();

            return eventosResposta;
        }
    }
}

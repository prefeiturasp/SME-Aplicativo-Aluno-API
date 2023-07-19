using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagensUsuarioLogadoAlunoQueryHandler : IRequestHandler<MensagensUsuarioLogadoAlunoQuery, IEnumerable<NotificacaoResposta>>
    {
        private readonly INotificacaoRepositorio notificacaoRepository;

        public MensagensUsuarioLogadoAlunoQueryHandler(INotificacaoRepositorio notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Handle(MensagensUsuarioLogadoAlunoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<NotificacaoResposta>();

            foreach (var item in request.Parametros)
            {
                var resultado = await notificacaoRepository
                    .ListarNotificacoes(item.ModalidadesId, item.TiposEscolas, item.CodigoUE, item.CodigoDRE, item.CodigoTurma, item.CodigoAluno, item.CodigoUsuario, item.SerieResumida);
                retorno.AddRange(resultado);
            }

            if (retorno != null || retorno.Any())
                return retorno;
            else 
                return default;
        }
    }
}

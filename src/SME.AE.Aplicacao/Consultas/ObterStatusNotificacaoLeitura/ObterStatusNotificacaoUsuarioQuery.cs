using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterStatusNotificacaoUsuarioQuery : IRequest<IEnumerable<StatusNotificacaoUsuario>>
    {
        public ObterStatusNotificacaoUsuarioQuery(List<long> notificoesId, long codigoAluno)
        {
            NotificoesId = notificoesId;
            CodigoAluno = codigoAluno;
        }

        public List<long> NotificoesId { get; set; }
        public long CodigoAluno { get; set; }
    }
}

using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class ListarNotificacaoAlunoQuery : IRequest<IEnumerable<NotificacaoResposta>>
    {
        public string Modalidades { get; set; }
        public string TiposEscolas { get; set; }
        public string CodigoUE { get; set; }
        public string CodigoDRE { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public long CodigoUsuario { get; set; }
        public string SerieResumida { get; set; }
    }
}

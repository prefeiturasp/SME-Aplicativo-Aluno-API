using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagensUsuarioLogadoAlunoQuery : IRequest<IEnumerable<NotificacaoResposta>>
    {
        public MensagensUsuarioLogadoAlunoQuery()
        {
            Parametros = new HashSet<ParametrosMensagensUsuarioLogado>();
        }
        public ICollection<ParametrosMensagensUsuarioLogado> Parametros { get; set; }
    }

    public class ParametrosMensagensUsuarioLogado
    {
        public string ModalidadesId { get; set; }
        public string TiposEscolas { get; set; }
        public string CodigoUE { get; set; }
        public string CodigoDRE { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public long CodigoUsuario { get; set; }
        public DateTime DataUltimaConsulta { get; set; }
        public string SerieResumida { get; set; }
    }
}

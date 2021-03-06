﻿using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagensUsuarioLogadoAlunoQuery : IRequest<IEnumerable<NotificacaoResposta>>
    {
        public string ModalidadesId { get; set; }
        public string CodigoUE { get; set; }
        public string CodigoDRE { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public long CodigoUsuario { get; set; }
        public DateTime DataUltimaConsulta { get; set; }
        public string SerieResumida { get; set; }
    }
}

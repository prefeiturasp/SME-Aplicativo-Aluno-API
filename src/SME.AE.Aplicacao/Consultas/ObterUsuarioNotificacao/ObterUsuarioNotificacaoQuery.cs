using MediatR;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioNotificacao
{
    public class ObterUsuarioNotificacaoQuery : IRequest<UsuarioNotificacao>
    {
        public long UsuarioId { get; set; }
        public long CodigoAluno { get; set; }
        public long NotificacaoId { get; set; }
    }
}

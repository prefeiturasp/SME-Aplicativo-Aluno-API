using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class UsuarioAlunoNotificacaoApp
    {
        public short AnoLetivo { get; set; }
        public long NotificacaoId { get; set; }
        public long CpfResponsavel { get; set; }
        public string CodigoAluno { get; set; }
    }
}

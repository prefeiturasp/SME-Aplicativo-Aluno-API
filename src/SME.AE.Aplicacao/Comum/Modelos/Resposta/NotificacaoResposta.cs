using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class NotificacaoResposta
    {
        public long Id { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<Grupo> Grupos { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public bool MensagemVisualizada { get; internal set; }
    }
}

using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class NotificacaoSgpDto : ModeloBase
    {
        public string AlteradoRF { get; set; }
        public string CriadoRF { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public string Grupo { get; set; }
        public IEnumerable<string> Alunos { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public IEnumerable<string> Turma { get; set; }
        public TipoComunicado TipoComunicado { get; set; }
        public string CategoriaNotificacao { get; set; }

        public void InserirCategoria()
        {
            if (TipoComunicado == TipoComunicado.SME)
                CategoriaNotificacao = "SME";
            else if (TipoComunicado == TipoComunicado.DRE ||
                     TipoComunicado == TipoComunicado.UE ||
                     TipoComunicado == TipoComunicado.UEMOD)
                CategoriaNotificacao = "UE";
            else if (TipoComunicado == TipoComunicado.TURMA ||
                     TipoComunicado == TipoComunicado.ALUNO)
                CategoriaNotificacao = "TURMA";
        }
    }
}

using SME.AE.Dominio.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
    public class Notificacao : EntidadeBase
    {

        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public string Grupo { get; set; }
        public IEnumerable<string> Alunos { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public IEnumerable<string> Turmas { get; set; }
        public TipoComunicado TipoComunicado { get; set; }
        public string CategoriaNotificacao { get; set; }
        
        public void InserirCategoria()
        {
            switch (TipoComunicado)
            {
                case TipoComunicado.SME:
                    CategoriaNotificacao = "SME";
                    break;
                case TipoComunicado.DRE:
                case TipoComunicado.UE:
                case TipoComunicado.UEMOD:
                    CategoriaNotificacao = "UE";
                    break;
                case TipoComunicado.TURMA:
                case TipoComunicado.ALUNO:
                    CategoriaNotificacao = "TURMA";
                    break;
                default:
                    break;
            }
        }
    }

}

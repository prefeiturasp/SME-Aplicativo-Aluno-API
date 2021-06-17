using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class NotificacaoResposta
    {
        public long Id { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public IEnumerable<Grupo> Grupos { get; set; }
        public string[] GruposId { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string AlteradoPor { get; set; }
        public IEnumerable<NotificacaoTurma> Turmas { get; set; }
        public bool MensagemVisualizada { get; internal set; }
        public TipoComunicado TipoComunicado { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string SeriesResumidas { get; set; }
        public string CategoriaNotificacao { 
            get {
                if (string.IsNullOrWhiteSpace(CodigoDre) && string.IsNullOrWhiteSpace(CodigoUe))
                {
                    return "SME";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(CodigoDre) && string.IsNullOrWhiteSpace(CodigoUe))
                    {
                        return "DRE";
                    }
                    else
                    {
                        return "UE";
                    }
                }
            }
            set { }
        }
    }



}

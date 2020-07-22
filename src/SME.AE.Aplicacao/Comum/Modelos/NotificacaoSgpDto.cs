using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class NotificacaoSgpDto : ModeloBase
    //{
    //    public string Mensagem { get; set; }
    //    public string Titulo { get; set; }
    //    public string Grupo { get; set; }
    //    public DateTime DataEnvio { get; set; }
    //    public DateTime? DataExpiracao { get; set; }

    {
       
        public string CriadoRF { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public string Grupo { get; set; }
        public IEnumerable<string> Alunos { get; set; }
        public long Id { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string Turma { get; set; }
       // public Modalidade? Modalidade { get; set; }
        public int? Semestre { get; set; }
        public TipoComunicadoEnum TipoComunicado { get; set; }
    }




}
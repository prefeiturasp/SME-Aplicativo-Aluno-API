using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class NotificacaoSgpDto : ModeloBase
    {
        public string AlteradoRF { get; set; }
        public string CriadoRF { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime DataExpiracao { get; set; }
        public string Grupo { get; set; }
        public IEnumerable<string> Alunos { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public int AnoLetivo { get; set; }
        public string SeriesResumidas { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public IEnumerable<string> Turmas { get; set; }
        public TipoComunicado TipoComunicado { get; set; }
        public string CategoriaNotificacao { get; set; }
        public bool EnviadoPushNotification { get; set; }
        public string Modalidades { get; set; }

        public void InserirCategoria()
        {
            if (string.IsNullOrWhiteSpace(CodigoDre) && string.IsNullOrWhiteSpace(CodigoUe))
            {
                CategoriaNotificacao = "SME";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(CodigoDre) && string.IsNullOrWhiteSpace(CodigoUe))
                {
                    CategoriaNotificacao = "DRE";
                }
                else
                {
                    CategoriaNotificacao = "UE";
                }
            }
        }

        public List<int> ObterGrupoLista() => Grupo.ToIntEnumerable().ToList();
        public IEnumerable<string> ObterSeriesResumidas() => SeriesResumidas.ToStringEnumerable();
    }
}

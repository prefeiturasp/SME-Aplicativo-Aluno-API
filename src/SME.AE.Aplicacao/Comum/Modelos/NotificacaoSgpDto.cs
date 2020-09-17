﻿using SME.AE.Aplicacao.Comum.Enumeradores;
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
        public DateTime? DataExpiracao { get; set; }
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

        public void InserirCategoria()
        {
            switch (TipoComunicado)
            {
                case TipoComunicado.SME:
                case TipoComunicado.SME_ANO:
                    CategoriaNotificacao = "SME";
                    break;
                case TipoComunicado.DRE:
                case TipoComunicado.DRE_ANO:
                    CategoriaNotificacao = "UE";
                    break;
                case TipoComunicado.UE:
                case TipoComunicado.UEMOD:
                    CategoriaNotificacao = String.IsNullOrEmpty(SeriesResumidas) ? "UE" : "TURMA";
                    break;
                case TipoComunicado.TURMA:
                case TipoComunicado.ALUNO:
                    CategoriaNotificacao = "TURMA";
                    break;
                default:
                    throw new NegocioException("Categoria não identificada");
            }
        }

        public List<int> ObterGrupoLista() => Grupo.ToIntEnumerable().ToList();
        public IEnumerable<string> ObterSeriesResumidas() => SeriesResumidas.ToStringEnumerable();
    }
}

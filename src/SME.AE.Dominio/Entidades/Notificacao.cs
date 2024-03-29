﻿using SME.AE.Dominio.Comum.Enumeradores;
using System;
using System.Collections.Generic;

namespace SME.AE.Dominio.Entidades
{
    public class Notificacao : EntidadeBase
    {

        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
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
        public string TiposEscolas { get; set; }

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
    }

}

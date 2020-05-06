﻿using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario
{
    public class NotificacaoPorUsuario
    {
        public long Id { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public string Grupo { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public bool MensagemVisualizada { get; set; }
    }
}

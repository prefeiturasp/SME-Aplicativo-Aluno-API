﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
    [Table("usuario_notificacao_leitura")]
    public class UsuarioNotificacao
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public long CodigoAlunoEol { get; set; }
        public long NotificacaoId { get; set; }
        public string  DreCodigoEol { get; set; }
        public string  UeCodigoEol { get; set; }
        public string UsuarioCpf { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string AlteradoPor { get; set; }
        public bool  MensagemVisualizada { get; set; }
    }
}

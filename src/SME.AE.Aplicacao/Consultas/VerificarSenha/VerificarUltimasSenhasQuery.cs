﻿using MediatR;
using System;

namespace SME.AE.Aplicacao.Consultas.VerificarSenha
{
    public class VerificarUltimasSenhasQuery : IRequest<bool>
    {
        public VerificarUltimasSenhasQuery()
        {

        }

        public VerificarUltimasSenhasQuery(Guid usuarioIdCore, string senhaCriptografada)
        {
            UsuarioIdCore = usuarioIdCore;
            SenhaCriptografada = senhaCriptografada;
        }

        public Guid UsuarioIdCore { get; set; }
        public string SenhaCriptografada { get; set; }
    }
}

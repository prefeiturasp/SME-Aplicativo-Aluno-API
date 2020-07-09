﻿using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RetornoUsuarioEol
    {
        private string _nome;

        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Nome
        {
            get
            {
                return TipoResponsavel == TipoResponsavelEnum.Proprio_Aluno && !string.IsNullOrWhiteSpace(NomeSocial) ? NomeSocial : _nome;
            }

            set => _nome = value;
        }
        public TipoResponsavelEnum TipoResponsavel { get; set; }
        public string NomeSocial { get; set; }
        public DateTime DataNascimento { get; set; }
        public int TipoSigilo { get; set; }
        public string DDD { get; set; }
        public string Celular { get; set; }
    }
}

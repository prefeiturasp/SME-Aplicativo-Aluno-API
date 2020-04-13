﻿using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class ListaEscola
    {
        public string DescricaoTipoEscola { get; set; }
        public int CodigoTipoEscola { get; set; }
        public IEnumerable<Aluno> Alunos { get; set; }
    }
}

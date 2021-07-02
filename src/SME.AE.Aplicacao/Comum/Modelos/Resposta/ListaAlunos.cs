using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class ListaEscola
    {
        public string Modalidade { get; set; }
        public int ModalidadeCodigo { get; set; }
        public IEnumerable<Aluno> Alunos { get; set; }
    }
}

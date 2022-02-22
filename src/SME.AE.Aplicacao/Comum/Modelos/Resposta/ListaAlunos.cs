using SME.AE.Dominio.Entidades;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class ListaEscola
    {
        public string Modalidade { get; set; }
        public int ModalidadeCodigo { get; set; }
        public IEnumerable<Aluno> Alunos { get; set; }
    }
}

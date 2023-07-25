using SME.AE.Aplicacao.Comum.Enumeradores;
using System;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class DadosResponsavelAlunoResumido
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public TipoResponsavelEnum TipoResponsavel { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string NomeMae { get; set; }
        public string DDDCelular { get; set; }
        public string NumeroCelular { get; set; }
        public string ObterCelularComDDD()
        {
            return $"{DDDCelular}{NumeroCelular}";
        }
    }
}

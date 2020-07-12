using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class AlterarSenhaDto
    {
        public AlterarSenhaDto()
        {

        }

        public AlterarSenhaDto(string cpf, string senha)
        {
            CPF = cpf;
            Senha = senha;
        }

        public string CPF { get; set; }
        public string Senha { get; set; }
    }
}

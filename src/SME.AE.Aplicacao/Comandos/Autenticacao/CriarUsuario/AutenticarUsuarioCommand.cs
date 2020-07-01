using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario
{
    public class AutenticarUsuarioCommand : IRequest<RespostaApi>
    {
        public AutenticarUsuarioCommand(string cpf, string senha)
        {
            Cpf = cpf;
            Senha = senha;

        }

        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; }       
    }
}

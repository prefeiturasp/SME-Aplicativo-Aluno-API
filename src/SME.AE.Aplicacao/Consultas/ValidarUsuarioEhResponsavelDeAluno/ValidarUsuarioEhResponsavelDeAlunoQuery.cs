using MediatR;

namespace SME.AE.Aplicacao.Consultas
{
    public class ValidarUsuarioEhResponsavelDeAlunoQuery : IRequest<bool>
    {
        public string Cpf { get; set; }

        public ValidarUsuarioEhResponsavelDeAlunoQuery(string cpf)
        {
            Cpf = cpf;
        }
    }
}

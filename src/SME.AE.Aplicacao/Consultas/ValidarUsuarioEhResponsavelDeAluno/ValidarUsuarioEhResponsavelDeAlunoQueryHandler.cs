using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ValidarUsuarioEhResponsavelDeAlunoQueryHandler : IRequestHandler<ValidarUsuarioEhResponsavelDeAlunoQuery, bool>
    {
        private readonly IAutenticacaoRepositorio autenticaacoRepositorio;

        public ValidarUsuarioEhResponsavelDeAlunoQueryHandler(IAutenticacaoRepositorio autenticaacoRepositorio)
        {
            this.autenticaacoRepositorio = autenticaacoRepositorio ?? throw new System.ArgumentNullException(nameof(autenticaacoRepositorio));
        }

        public async Task<bool> Handle(ValidarUsuarioEhResponsavelDeAlunoQuery request, CancellationToken cancellationToken)
        {
            var alunosDoResponsavel = await autenticaacoRepositorio.SelecionarAlunosResponsavel(request.Cpf);
            if (alunosDoResponsavel == null || !alunosDoResponsavel.Any())
                return false;
            return true;
        }
    }
}

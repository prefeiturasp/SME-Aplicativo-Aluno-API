using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryHandler : IRequestHandler<ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery, bool>
    {
        private readonly IAceiteTermosDeUsoRepositorio _aceiteTermosDeUsoRepositorio;

        public ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryHandler(IAceiteTermosDeUsoRepositorio aceiteTermosDeUsoRepositorio)
        {
            _aceiteTermosDeUsoRepositorio = aceiteTermosDeUsoRepositorio ?? throw new System.ArgumentNullException(nameof(aceiteTermosDeUsoRepositorio));
        }

        public async Task<bool> Handle(ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _aceiteTermosDeUsoRepositorio.ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(request.CpfUsuario, request.Versao);
            if (retorno)
                throw new NegocioException("Este usuário já aceitou os termos de uso!");

            return retorno;
        }
    }
}

using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterNomeComponenteCurricularQueryHandler : IRequestHandler<ObterNomeComponenteCurricularQuery, string>
    {
        private readonly IComponenteCurricularSgpRepositorio componenteCurricularSgpRepositorio;

        public ObterNomeComponenteCurricularQueryHandler(IComponenteCurricularSgpRepositorio componenteCurricularSgpRepositorio)
        {
            this.componenteCurricularSgpRepositorio = componenteCurricularSgpRepositorio ?? throw new ArgumentNullException(nameof(componenteCurricularSgpRepositorio));
        }
        public async Task<string> Handle(ObterNomeComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            return await componenteCurricularSgpRepositorio
                .ObterDescricaoComponenteCurricular(request.CodigoComponenteCurricular);
        }
    }
}

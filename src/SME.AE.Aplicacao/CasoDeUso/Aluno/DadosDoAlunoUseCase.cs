using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;

namespace SME.AE.Aplicacao.CasoDeUso.Aluno
{
    public class DadosDoAlunoUseCase : IDadosDoAlunoUseCase
    {
        private readonly IMediator mediator;

        public DadosDoAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(string cpf)
        {
            var resposta = await mediator.Send(new DadosAlunoCommand(cpf));
            return resposta;
        }
    }
}

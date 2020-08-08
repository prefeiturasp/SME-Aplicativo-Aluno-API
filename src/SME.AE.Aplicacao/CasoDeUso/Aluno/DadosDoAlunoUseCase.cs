using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Sentry;
using Xunit.Sdk;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Comum.Excecoes;

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
            RespostaApi resposta = await mediator.Send(new DadosAlunoCommand(cpf));

            if (!resposta.Ok)
                throw new NegocioException(string.Join(',', resposta.Erros));

            return resposta;
        }
    }
}

using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioEolUseCase : IAtualizarDadosUsuarioEolUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioEolUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {
            var dadosMensagem = mensagemRabbit.ObterObjetoMensagem<AtualizarDadosUsuarioDto>();

            if (dadosMensagem == null)
            {
                SentrySdk.CaptureMessage($"Não foi possível realizar a atualização dos dados do responsável do aluno no eol", Sentry.Protocol.SentryLevel.Error);
            }

            // TODO : Obter alunos do responsável 

            // Usar transação

            // TODO : Atualizar os dados dos alunos

            // TODO : Atualizar os dados do responsável
        }
    }
}

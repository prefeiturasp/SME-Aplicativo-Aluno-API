using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioNotificacao;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class MarcarExcluidaMensagenUsuarioAlunoIdUseCase : IMarcarExcluidaMensagenUsuarioAlunoIdUseCase
    {
        private readonly IMediator mediator;

        public MarcarExcluidaMensagenUsuarioAlunoIdUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(string cpf, long codigoAluno, long id)
        {
            return await mediator.Send(new MarcarExcluidaMensagenUsuarioAlunoIdCommand{ Cpf = cpf, CodigoAluno = codigoAluno, NotificacaoId = id });
        }
    }
}

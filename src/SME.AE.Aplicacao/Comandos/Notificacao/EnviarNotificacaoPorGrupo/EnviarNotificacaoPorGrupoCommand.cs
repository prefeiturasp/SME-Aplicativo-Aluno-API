using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommand : IRequest<bool>
    {
        public SME.AE.Dominio.Entidades.Notificacao Notificacao { set; get; }

        public EnviarNotificacaoPorGrupoCommand(SME.AE.Dominio.Entidades.Notificacao notificacao)
        {
            this.Notificacao = notificacao;
        }
    }

    public class EnviarNotificacaoPorGrupoCommandHandler : IRequestHandler<EnviarNotificacaoPorGrupoCommand, bool>
    {
        private readonly INotificacaoRepository _repository;

        public EnviarNotificacaoPorGrupoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(EnviarNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {

            return false;
        }
    }
}

using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MediatR;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommand : IRequest<bool>
    {
      
        public Message Mensagem { get; set; }

        public EnviarNotificacaoPorGrupoCommand(Message mensagem)
        {
            this.Mensagem = mensagem;
        }
    }

    public class EnviarNotificacaoPorGrupoCommandHandler : IRequestHandler<EnviarNotificacaoPorGrupoCommand, bool>
    {
        private readonly IGrupoComunicadoRepository _repositorioGrupoComunicado;
        public EnviarNotificacaoPorGrupoCommandHandler(IGrupoComunicadoRepository repositorioGrupoComunicado)
        {
            _repositorioGrupoComunicado = repositorioGrupoComunicado;
        }

        public async Task<bool> Handle(EnviarNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            var firebaseCredential = GoogleCredential.FromJson(VariaveisAmbiente.FirebaseToken);

            FirebaseApp app = FirebaseApp.DefaultInstance;
            String resultado = "";

            if (app == null)
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = firebaseCredential,
                    ProjectId = VariaveisAmbiente.FirebaseProjectId
                });
            }
            try
            {
                resultado = await FirebaseMessaging.DefaultInstance.SendAsync(request.Mensagem).ConfigureAwait(true);
                return resultado != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
        }
    }
}


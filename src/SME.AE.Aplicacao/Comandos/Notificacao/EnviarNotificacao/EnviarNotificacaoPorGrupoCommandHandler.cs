using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MediatR;
using SME.AE.Aplicacao.Comum.Config;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommandHandler : IRequestHandler<EnviarNotificacaoPorGrupoCommand, bool>
    {
        public async Task<bool> Handle(EnviarNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            var firebaseToken = VariaveisAmbiente.FirebaseToken;
            var firebaseCredential = GoogleCredential.FromJson(firebaseToken);
            FirebaseApp app = FirebaseApp.DefaultInstance;

            if (app == null)
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = firebaseCredential,
                    ProjectId = VariaveisAmbiente.FirebaseProjectId
                });
            }
            var resultado = await FirebaseMessaging.DefaultInstance.SendAsync(request.Mensagem).ConfigureAwait(true);
            return resultado != null;
        }
    }
}


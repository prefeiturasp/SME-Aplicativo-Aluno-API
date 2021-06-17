using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MediatR;
using SME.AE.Comum;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommandHandler : IRequestHandler<EnviarNotificacaoPorGrupoCommand, bool>
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public EnviarNotificacaoPorGrupoCommandHandler(VariaveisGlobaisOptions variaveisGlobaisOptions )
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new System.ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        public async Task<bool> Handle(EnviarNotificacaoPorGrupoCommand request, CancellationToken cancellationToken)
        {
            var firebaseToken = variaveisGlobaisOptions.FirebaseToken;
            var firebaseCredential = GoogleCredential.FromJson(firebaseToken);
            FirebaseApp app = FirebaseApp.DefaultInstance;

            if (app == null)
            {
                app = FirebaseApp.Create(new AppOptions()
                {
                    Credential = firebaseCredential,
                    ProjectId = variaveisGlobaisOptions.FirebaseProjectId
                });
            }
            var resultado = await FirebaseMessaging.DefaultInstance.SendAsync(request.Mensagem).ConfigureAwait(true);
            return resultado != null;
        }
    }
}


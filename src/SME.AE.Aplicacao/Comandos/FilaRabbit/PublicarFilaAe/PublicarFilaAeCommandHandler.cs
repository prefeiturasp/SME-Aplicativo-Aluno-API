using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using RabbitMQ.Client;
using SME.AE.Comum;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class PublicarFilaAeCommandHandler : IRequestHandler<PublicarFilaAeCommand, bool>
    {
        private readonly IAsyncPolicy policy;
        private readonly ConnectionFactory connectionFactory;

        public PublicarFilaAeCommandHandler(IReadOnlyPolicyRegistry<string> registry, ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task<bool> Handle(PublicarFilaAeCommand command, CancellationToken cancellationToken)
        {
            var request = new MensagemRabbit(command.Mensagem, command.CodigoCorrelacao);

            var mensagem = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var body = Encoding.UTF8.GetBytes(mensagem);

            await policy.ExecuteAsync(() => PublicarMensagem(command.Rota, body));

            return true;
        }

        private async Task PublicarMensagem(string rota, byte[] body)
        {
            using var conexaoRabbit = connectionFactory.CreateConnection();
            using IModel _channel = conexaoRabbit.CreateModel();
            _channel.BasicPublish(ExchangeRabbit.Ae, rota, null, body);
        }
    }
}

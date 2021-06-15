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
    public class PublicarFilaEolCommandHandler : IRequestHandler<PublicarFilaEolCommand, bool>
    {
        private readonly IConfiguration configuration;
        private readonly IAsyncPolicy policy;

        public PublicarFilaEolCommandHandler(IConfiguration configuration, IReadOnlyPolicyRegistry<string> registry)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.policy = registry.Get<IAsyncPolicy>(PoliticaPolly.PublicaFila);
        }

        public async Task<bool> Handle(PublicarFilaEolCommand command, CancellationToken cancellationToken)
        {
            var request = new MensagemRabbit(command.Filtros,
                                             command.CodigoCorrelacao,
                                             command.UsuarioCpf,
                                             command.UsuarioNome);

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
            var factory = new ConnectionFactory
            {
                HostName = configuration.GetSection("ConfiguracaoRabbit:HostName").Value,
                UserName = configuration.GetSection("ConfiguracaoRabbit:UserName").Value,
                Password = configuration.GetSection("ConfiguracaoRabbit:Password").Value,
                VirtualHost = configuration.GetSection("ConfiguracaoRabbit:Virtualhost").Value
            };

            using var conexaoRabbit = factory.CreateConnection();
            using IModel _channel = conexaoRabbit.CreateModel();
            _channel.BasicPublish(ExchangeRabbit.Eol, rota, null, body);
        }
    }
}

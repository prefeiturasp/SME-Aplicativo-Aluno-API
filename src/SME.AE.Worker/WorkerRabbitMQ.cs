using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;
using Sentry.Protocol;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Comum;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Fila;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Worker
{
    public class WorkerRabbitMQ : IHostedService
    {
        private readonly IModel canalRabbit;
        private readonly IConnection conexaoRabbit;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// configuração da lista de tipos para a fila do rabbit instanciar, seguindo a ordem de propriedades:
        /// rota do rabbit, usaMediatr?, tipo
        /// </summary>
        private readonly Dictionary<string, ComandoRabbit> comandos;

        public WorkerRabbitMQ(IServiceScopeFactory serviceScopeFactory, ConnectionFactory connRabbitFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            conexaoRabbit = connRabbitFactory.CreateConnection();
            canalRabbit = conexaoRabbit.CreateModel();

            canalRabbit.BasicQos(0, 10, false);

            canalRabbit.ExchangeDeclare(ExchangeRabbit.Ae, ExchangeType.Direct, true, false);
            canalRabbit.ExchangeDeclare(ExchangeRabbit.AeDeadLetter, ExchangeType.Direct, true, false);

            DeclararFilasAe();

            comandos = new Dictionary<string, ComandoRabbit>();
            RegistrarUseCases();
        }

        private void DeclararFilasAe()
        {
            foreach (var fila in typeof(RotasRabbitAe).ObterConstantesPublicas<string>())
            {
                var args = new Dictionary<string, object>()
                    {
                        { "x-dead-letter-exchange", ExchangeRabbit.AeDeadLetter }
                    };

                canalRabbit.QueueDeclare(fila, true, false, false, args);
                canalRabbit.QueueBind(fila, ExchangeRabbit.Ae, fila, null);

                var filaDeadLetter = $"{fila}.deadletter";
                canalRabbit.QueueDeclare(filaDeadLetter, true, false, false, null);
                canalRabbit.QueueBind(filaDeadLetter, ExchangeRabbit.AeDeadLetter, fila, null);
            }
        }

        private void RegistrarUseCases()
        {
            comandos.Add(RotasRabbitAe.RotaAtualizacaoCadastralProdam, new ComandoRabbit("Atualizar Cadastro de Usuário na Prodam", typeof(IAtualizarDadosUsuarioProdamUseCase)));
            comandos.Add(RotasRabbitAe.RotaAtualizacaoCadastralEol, new ComandoRabbit("Atualizar Cadastro de Usuário no Eol", typeof(IAtualizarDadosUsuarioEolUseCase)));
        }

        private async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var mensagem = Encoding.UTF8.GetString(ea.Body.Span);
            var rota = ea.RoutingKey;
            if (comandos.ContainsKey(rota))
            {
                var mensagemRabbit = JsonConvert.DeserializeObject<MensagemRabbit>(mensagem);
                var comandoRabbit = comandos[rota];
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();

                    var casoDeUso = scope.ServiceProvider.GetService(comandoRabbit.TipoCasoUso);

                    await ObterMetodo(comandoRabbit.TipoCasoUso, "Executar").InvokeAsync(casoDeUso, new object[] { mensagemRabbit });
                    canalRabbit.BasicAck(ea.DeliveryTag, false);
                }
                catch (NegocioException nex)
                {
                    canalRabbit.BasicAck(ea.DeliveryTag, false);
                    SentrySdk.AddBreadcrumb($"Erros: {nex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(nex);
                    RegistrarSentry(ea, mensagemRabbit, nex);
                }
                catch (ValidacaoException vex)
                {
                    canalRabbit.BasicAck(ea.DeliveryTag, false);
                    SentrySdk.CaptureException(vex);
                    RegistrarSentry(ea, mensagemRabbit, vex);
                }
                catch (Exception ex)
                {
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                    SentrySdk.AddBreadcrumb($"Erros: {ex.Message}", null, null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    RegistrarSentry(ea, mensagemRabbit, ex);
                }

            }
            else
                canalRabbit.BasicReject(ea.DeliveryTag, false);
        }

        private static void RegistrarSentry(BasicDeliverEventArgs ea, MensagemRabbit mensagemRabbit, Exception ex)
        {
            SentrySdk.CaptureMessage($"{mensagemRabbit.CodigoCorrelacao.ToString().Substring(0, 3)} - ERRO - {ea.RoutingKey}", SentryLevel.Error);
            SentrySdk.CaptureException(ex);
        }
        private MethodInfo ObterMetodo(Type objType, string method)
        {
            var executar = objType.GetMethod(method);

            if (executar == null)
            {
                foreach (var itf in objType.GetInterfaces())
                {
                    executar = ObterMetodo(itf, method);
                    if (executar != null)
                        break;
                }
            }

            return executar;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canalRabbit.Close();
            conexaoRabbit.Close();
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(canalRabbit);

            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    await TratarMensagem(ea);
                }
                catch (Exception ex)
                {
                    SentrySdk.AddBreadcrumb($"Erro ao tratar mensagem {ea.DeliveryTag}", "erro", null, null, BreadcrumbLevel.Error);
                    SentrySdk.CaptureException(ex);
                    canalRabbit.BasicReject(ea.DeliveryTag, false);
                }
            };

            RegistrarConsumerAe(consumer);
            return Task.CompletedTask;
        }

        private void RegistrarConsumerAe(EventingBasicConsumer consumer)
        {
            foreach (var fila in typeof(RotasRabbitAe).ObterConstantesPublicas<string>())
                canalRabbit.BasicConsume(fila, false, consumer);
        }
    }
}

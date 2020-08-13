using MailKit.Net.Smtp;
using MimeKit;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Servicos
{
    public class EmailServico : IEmailServico
    {
        private readonly IConfiguracaoEmailRepositorio repositorioConfiguracaoEmail;

        public EmailServico(IConfiguracaoEmailRepositorio repositorioConfiguracaoEmail)
        {
            this.repositorioConfiguracaoEmail = repositorioConfiguracaoEmail ?? throw new System.ArgumentNullException(nameof(repositorioConfiguracaoEmail));
        }

        public async Task Enviar(string nomeDestinatario,string destinatario, string assunto, string mensagemHtml)
        {
            ConfiguracaoEmail configuracaoEmail = await ObterConfiguracoes();

            var message = Montarmensagem(nomeDestinatario, destinatario, assunto, mensagemHtml, configuracaoEmail);

            await ExecutarEnvio(configuracaoEmail, message);
        }

        private static async Task ExecutarEnvio(ConfiguracaoEmail configuracaoEmail, MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(configuracaoEmail.ServidorSmtp, configuracaoEmail.Porta, configuracaoEmail.UsarTls);

                await client.AuthenticateAsync(configuracaoEmail.Usuario, configuracaoEmail.Senha);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }

        private static MimeMessage Montarmensagem(string nomeDestinatario, string destinatario, string assunto, string mensagemHtml, ConfiguracaoEmail configuracaoEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuracaoEmail.NomeRemetente, configuracaoEmail.EmailRemetente));
            message.To.Add(new MailboxAddress(nomeDestinatario, destinatario));
            message.Subject = assunto;

            message.Body = new TextPart("html")
            {
                Text = mensagemHtml
            };
            return message;
        }

        private async Task<ConfiguracaoEmail> ObterConfiguracoes()
        {
            var configuracoes = await repositorioConfiguracaoEmail.ListarAsync();

            var configuracaoEmail = configuracoes?.FirstOrDefault() ?? throw new NegocioException("Não foi possível recuperar as configurações para envio de e-mail.");
            return configuracaoEmail;
        }
    }
}

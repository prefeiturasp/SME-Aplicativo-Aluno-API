using MailKit.Net.Smtp;
using MimeKit;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Servicos
{
    public class EmailServico : IEmailServico
    {
        private readonly ConfiguracaoEmail configuracaoEmail;

        public EmailServico(IConfiguracaoEmailRepositorio repositorioConfiguracaoEmail)
        {
            if (repositorioConfiguracaoEmail is null)
                throw new System.ArgumentNullException(nameof(repositorioConfiguracaoEmail));

            var configuracoes = repositorioConfiguracaoEmail.Listar();

            configuracaoEmail = configuracoes?.FirstOrDefault() ?? throw new NegocioException("Não foi possível recuperar as configurações para envio de e-mail.");
        }

        public async Task Enviar(string nomeDestinatario,string destinatario, string assunto, string mensagemHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuracaoEmail.NomeRemetente, configuracaoEmail.EmailRemetente));
            message.To.Add(new MailboxAddress(nomeDestinatario,destinatario));
            message.Subject = assunto;

            message.Body = new TextPart("html")
            {
                Text = mensagemHtml
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(configuracaoEmail.ServidorSmtp, configuracaoEmail.Porta, configuracaoEmail.UsarTls);

                await client.AuthenticateAsync(configuracaoEmail.Usuario, configuracaoEmail.Senha);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}

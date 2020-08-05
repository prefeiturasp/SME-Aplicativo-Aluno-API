using MailKit.Net.Smtp;
using MimeKit;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Linq;

namespace SME.AE.Aplicacao.Servicos
{
    public class EmailServico
    {
        private readonly ConfiguracaoEmail configuracaoEmail;

        public EmailServico(IConfiguracaoEmailRepositorio repositorioConfiguracaoEmail)
        {
            if (repositorioConfiguracaoEmail is null)
                throw new System.ArgumentNullException(nameof(repositorioConfiguracaoEmail));

            var configuracoes = repositorioConfiguracaoEmail.Listar();

            configuracaoEmail = configuracoes?.FirstOrDefault() ?? throw new NegocioException("Não foi possível recuperar as configurações para envio de e-mail.");
        }

        public void Enviar(string nomeDestinatario,string destinatario, string assunto, string mensagemHtml)
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
                client.Connect(configuracaoEmail.ServidorSmtp, configuracaoEmail.Porta, configuracaoEmail.UsarTls);

                client.Authenticate(configuracaoEmail.Usuario, configuracaoEmail.Senha);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}

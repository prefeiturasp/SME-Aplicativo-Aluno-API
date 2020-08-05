using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Servico.Servicos
{
    public class EmailServico
    {
        private readonly ConfiguracaoEmail configuracaoEmail;

        public EmailServico(IRepositorioConfiguracaoEmail repositorioConfiguracaoEmail)
        {
            if (repositorioConfiguracaoEmail is null)
            {
                throw new System.ArgumentNullException(nameof(repositorioConfiguracaoEmail));
            }
            var configuracoes = repositorioConfiguracaoEmail.Listar();
            if (configuracoes != null)
            {
                configuracaoEmail = configuracoes.FirstOrDefault();
            }
            else
            {
                throw new NegocioException("Não foi possível recuperar as configurações para envio de e-mail.");
            }
        }

        public void Enviar(string destinatario, string assunto, string mensagemHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuracaoEmail.NomeRemetente, configuracaoEmail.EmailRemetente));
            message.To.Add(new MailboxAddress(destinatario));
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

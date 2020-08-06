using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Interfaces.Servicos
{
    public interface IEmailServico
    {
        void Enviar(string nomeDestinatario, string destinatario, string assunto, string mensagemHtml);
    }
}

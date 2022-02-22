using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Servicos
{
    public interface IEmailServico
    {
        Task Enviar(string nomeDestinatario, string destinatario, string assunto, string mensagemHtml);
    }
}

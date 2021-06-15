using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Comum
{
   public class MensagemRabbit
    {
        public MensagemRabbit(string action, object mensagem, Guid codigoCorrelacao, string usuarioLogadoCpf)
        {
            Action = action;
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoCpf = usuarioLogadoCpf;
        }

        public MensagemRabbit(object mensagem, Guid codigoCorrelacao, string usuarioLogadoCpf, string usuarioLogadoNome)
        {
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoNomeCompleto = usuarioLogadoNome;
            UsuarioLogadoCpf = usuarioLogadoCpf;
        }

        public MensagemRabbit(object mensagem)
        {
            Mensagem = mensagem;
        }

        protected MensagemRabbit()
        {

        }
        public string Action { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioLogadoNomeCompleto { get; set; }
        public string UsuarioLogadoCpf { get; set; }

        public T ObterObjetoMensagem<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Mensagem.ToString());
        }
    }
}

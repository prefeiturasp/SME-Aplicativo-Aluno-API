using MediatR;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comandos.Logs
{
    public class SalvarLogErroCommand : IRequest<bool>
    {
        public Exception Excecao { get; }
        public string MensagemCustomizada { get; }
        public Dictionary<string, string> Labels { get; }
        public SalvarLogErroCommand(Exception excecao, string mensagemCustomizada = null, Dictionary<string, string> labels = null)
        {
            Excecao = excecao;
            MensagemCustomizada = mensagemCustomizada;
            Labels = labels ?? new Dictionary<string, string>();
        }
    }
}

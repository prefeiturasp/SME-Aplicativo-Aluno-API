using MediatR;
using System;

namespace SME.AE.Aplicacao
{
    public class PublicarFilaEolCommand : IRequest<bool>
    {
        public PublicarFilaEolCommand(string rota, object filtros, Guid codigoCorrelacao, string usuarioCpf, string usuarioNome)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioNome = usuarioNome;
            UsuarioCpf = usuarioCpf;
            Rota = rota;
        }

        public string Rota { get; set; }
        public object Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioCpf { get; set; }
    }
}

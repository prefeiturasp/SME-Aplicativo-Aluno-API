using MediatR;
using System;

namespace SME.AE.Aplicacao
{
    public class PublicarFilaEolCommand : IRequest<bool>
    {
        public PublicarFilaEolCommand(string rota, object filtros, Guid codigoCorrelacao, string cpf, string nome)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoNomeCompleto = nome;
            UsuarioLogadoCpf = cpf;
            Rota = rota;
        }

        public string Rota { get; set; }
        public object Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioLogadoNomeCompleto { get; set; }
        public string UsuarioLogadoCpf { get; set; }
    }
}

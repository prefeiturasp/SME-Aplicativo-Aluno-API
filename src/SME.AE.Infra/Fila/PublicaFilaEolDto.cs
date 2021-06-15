using SME.AE.Dominio.Entidades;
using System;

namespace SME.AE.Infra
{
   public class PublicaFilaEolDto
    {
        public PublicaFilaEolDto(string nomeFila, object filtros, Guid codigoCorrelacao, string cpf, string nome)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoNomeCompleto = nome;
            UsuarioLogadoCpf = cpf;
            NomeFila = nomeFila;
        }

        public string NomeFila { get; private set; }
        public object Filtros { get; private set; }
        public Guid CodigoCorrelacao { get; private set; }
        public string UsuarioLogadoNomeCompleto { get; private set; }
        public string UsuarioLogadoCpf { get; private set; }
    }
}

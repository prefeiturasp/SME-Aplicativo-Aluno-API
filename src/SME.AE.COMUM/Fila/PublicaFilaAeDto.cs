using System;

namespace SME.AE.Comum
{
    public class PublicaFilaAeDto
    {
        public PublicaFilaAeDto(string nomeFila, object mensagem, Guid codigoCorrelacao)
        {
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao;
            NomeFila = nomeFila;
        }

        public string NomeFila { get; private set; }
        public object Mensagem { get; private set; }
        public Guid CodigoCorrelacao { get; private set; }
    }
}

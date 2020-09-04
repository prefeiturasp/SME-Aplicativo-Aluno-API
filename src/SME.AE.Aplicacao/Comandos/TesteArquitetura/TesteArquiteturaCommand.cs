using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Comandos.TesteArquitetura
{
    public class TesteArquiteturaCommand : IRequest
    {
        public Aplicacao.Comum.Modelos.Entrada.UsuarioDto Usuario { get; set; } = new Comum.Modelos.Entrada.UsuarioDto();
        public string Cpf { get; set; }
        public string Id { get; set; }
    }
}

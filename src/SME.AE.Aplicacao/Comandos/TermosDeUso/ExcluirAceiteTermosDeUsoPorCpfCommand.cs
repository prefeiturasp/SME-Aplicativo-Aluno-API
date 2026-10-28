using MediatR;

namespace SME.AE.Aplicacao.Comandos.TermosDeUso
{
    public class ExcluirAceiteTermosDeUsoPorCpfCommand : IRequest<bool>
    {
        public string CpfUsuario { get; set; }

        public ExcluirAceiteTermosDeUsoPorCpfCommand(string cpfUsuario)
        {
            CpfUsuario = cpfUsuario;
        }
    }
}


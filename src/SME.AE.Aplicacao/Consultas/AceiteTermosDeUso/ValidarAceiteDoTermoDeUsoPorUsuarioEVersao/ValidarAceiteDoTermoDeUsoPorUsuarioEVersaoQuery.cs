using MediatR;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery : IRequest<bool>
    {
        public string CpfUsuario { get; set; }
        public double Versao { get; set; }

        public ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(string cpfUsuario, double versao)
        {
            CpfUsuario = cpfUsuario;
            Versao = versao;
        }
    }
}

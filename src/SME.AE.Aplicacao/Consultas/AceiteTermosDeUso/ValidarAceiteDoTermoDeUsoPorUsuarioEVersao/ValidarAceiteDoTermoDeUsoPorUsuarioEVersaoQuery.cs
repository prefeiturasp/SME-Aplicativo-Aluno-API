using MediatR;

namespace SME.AE.Aplicacao.Consultas.TermosDeUso
{
    public class ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery : IRequest<bool>
    {
        public string Usuario { get; set; }
        public double Versao { get; set; }

        public ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(string usuario, double versao)
        {
            Usuario = usuario;
            Versao = versao;
        }
    }
}

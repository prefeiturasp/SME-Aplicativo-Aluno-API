using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterModalidadeDeEnsinoQuery : IRequest<TurmaModalidadeDeEnsinoDto>
    {
        public string CodigoTurma { get; set; }

        public ObterModalidadeDeEnsinoQuery(string codigoTurma)
        {
            CodigoTurma = codigoTurma;
        }
    }
}

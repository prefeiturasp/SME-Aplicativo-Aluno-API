using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

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

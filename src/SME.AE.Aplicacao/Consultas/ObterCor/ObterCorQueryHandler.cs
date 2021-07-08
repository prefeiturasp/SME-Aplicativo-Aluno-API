using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterCorQueryHandler : IRequestHandler<ObterCorQuery, string>
    {
        public ObterCorQueryHandler()
        {
        }

        public async Task<string> Handle(ObterCorQuery request, CancellationToken cancellationToken)
        {
            string cor = null;

            if (request.Modalidade == ModalidadeDeEnsino.Infantil)
            {
                if (!request.FrequenciaAlunoCores.Any() || !request.FrequenciaAlunoFaixas.Any()) return FrequenciaAlunoCor.CorPadrao;
                var frequenciaEmAlertaFaixa = request.FrequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.EnsinoInfantilFrequenciaEmAlertaFaixa)?.Faixa;
                var frequenciaRegularFaixa = request.FrequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.EnsinoInfantilFrequenciaRegularFaixa)?.Faixa;

                cor = Math.Round((decimal)request.Frequencia, 2) switch
                {
                    decimal n when (n < frequenciaEmAlertaFaixa.GetValueOrDefault() / 100) => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaInsuficienteCor)?.Cor,
                    decimal n when (n < frequenciaRegularFaixa.GetValueOrDefault() / 100) => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaEmAlertaCor)?.Cor,
                    _ => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.EnsinoInfantilFrequenciaRegularCor)?.Cor,
                };
            }
            else
            {
                if (!request.FrequenciaAlunoCores.Any() || !request.FrequenciaAlunoFaixas.Any()) return FrequenciaAlunoCor.CorPadrao;
                var frequenciaEmAlertaFaixa = request.FrequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaEmAlertaFaixa)?.Faixa;
                var frequenciaRegularFaixa = request.FrequenciaAlunoFaixas.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoFaixa.FrequenciaRegularFaixa)?.Faixa;

                cor = Math.Round((decimal)request.Frequencia, 2) switch
                {
                    decimal n when (n < frequenciaEmAlertaFaixa.GetValueOrDefault() / 100) => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaInsuficienteCor)?.Cor,
                    decimal n when (n < frequenciaRegularFaixa.GetValueOrDefault() / 100) => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaEmAlertaCor)?.Cor,
                    _ => request.FrequenciaAlunoCores.FirstOrDefault(x => x.Frequencia == FrequenciaAlunoCor.FrequenciaRegularCor)?.Cor,
                };
            }

            return await Task.FromResult(cor ?? FrequenciaAlunoCor.CorPadrao);
        }
    }
}

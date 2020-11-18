using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno
{
    public class FrequenciaAlunoFaixa
    {
        public const string EnsinoInfantilFrequenciaEmAlertaFaixa = "EnsinoInfantilFrequenciaEmAlertaFaixa";
        public const string EnsinoInfantilFrequenciaRegularFaixa = "EnsinoInfantilFrequenciaRegularFaixa";

        public const string FrequenciaEmAlertaFaixa = "FrequenciaEmAlertaFaixa";
        public const string FrequenciaRegularFaixa = "FrequenciaRegularFaixa";

        public string Frequencia { get; set; }
        public decimal Faixa { get; set; }

        public static IEnumerable<string> ObterChavesDosParametrosParaEnsinoInfantil()
        {
            yield return EnsinoInfantilFrequenciaEmAlertaFaixa;
            yield return EnsinoInfantilFrequenciaRegularFaixa;
        }

        public static IEnumerable<string> ObterChavesDosParametros()
        {
            yield return FrequenciaEmAlertaFaixa;
            yield return FrequenciaRegularFaixa;
        }
    }
}
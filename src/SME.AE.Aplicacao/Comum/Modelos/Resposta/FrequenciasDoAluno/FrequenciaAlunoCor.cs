using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno
{
    public class FrequenciaAlunoCor
    {
        public const string EnsinoInfantilFrequenciaInsuficienteCor = "EnsinoInfantilFrequenciaInsuficienteCor";
        public const string EnsinoInfantilFrequenciaEmAlertaCor = "EnsinoInfantilFrequenciaEmAlertaCor";
        public const string EnsinoInfantilFrequenciaRegularCor = "EnsinoInfantilFrequenciaRegularCor";

        public const string FrequenciaInsuficienteCor = "FrequenciaInsuficienteCor";
        public const string FrequenciaEmAlertaCor = "FrequenciaEmAlertaCor";
        public const string FrequenciaRegularCor = "FrequenciaRegularCor";

        public const string CorPadrao = "#D4D4D4";

        public string Frequencia { get; set; }

        private string _cor;

        public string Cor
        {
            get => ValidarCor() ? _cor : CorPadrao;
            set => _cor = value;
        }

        private bool ValidarCor() => Regex.IsMatch(_cor, "^#((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$");

        public static IEnumerable<string> ObterChavesDosParametrosParaEnsinoInfantil()
        {
            yield return EnsinoInfantilFrequenciaInsuficienteCor;
            yield return EnsinoInfantilFrequenciaEmAlertaCor;
            yield return EnsinoInfantilFrequenciaRegularCor;
        }

        public static IEnumerable<string> ObterChavesDosParametros()
        {
            yield return FrequenciaInsuficienteCor;
            yield return FrequenciaEmAlertaCor;
            yield return FrequenciaRegularCor;
        }
    }
}
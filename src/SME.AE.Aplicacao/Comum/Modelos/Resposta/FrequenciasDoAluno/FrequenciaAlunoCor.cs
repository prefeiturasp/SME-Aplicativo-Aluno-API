using System.Text.RegularExpressions;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno
{
    public class FrequenciaAlunoCor
    {
        public const string FrequenciaInsuficiente = "FrequenciaInsuficiente";
        public const string FrequenciaEmAlerta = "FrequenciaEmAlerta";
        public const string FrequenciaRegular = "FrequenciaRegular";
        public const string CorPadrao = "#D4D4D4";

        public string Frequencia { get; set; }

        private string _cor;

        public string Cor
        {
            get => ValidarCor() ? _cor : CorPadrao;
            set => _cor = value;
        }

        private bool ValidarCor() => Regex.IsMatch(_cor, "^#((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$");
    }
}
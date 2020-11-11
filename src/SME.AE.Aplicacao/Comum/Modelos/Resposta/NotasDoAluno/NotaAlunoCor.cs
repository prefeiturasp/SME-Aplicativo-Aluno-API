using System.Text.RegularExpressions;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno
{
    public class NotaAlunoCor
    {
        public const string NotaAbaixo5 = "NotaAbaixo5";
        public const string NotaEntre7e5 = "NotaEntre7e5";
        public const string NotaAcimaDe7 = "NotaAcimaDe7";

        public const string CorPadrao = "#D4D4D4";

        public string Nota { get; set; }

        private string _cor;
        public string Cor
        {
            get => ValidarCor() ? _cor : CorPadrao;
            set => _cor = value;
        }

        private bool ValidarCor() => Regex.IsMatch(_cor, "^#((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$");
    }
}
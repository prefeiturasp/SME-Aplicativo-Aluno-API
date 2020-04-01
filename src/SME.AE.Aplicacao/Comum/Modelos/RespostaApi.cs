using System.Collections.Generic;
using System.Linq;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RespostaApi
    {
        internal RespostaApi(bool ok, IEnumerable<string> erros)
        {
            Ok = ok;
            Erros = erros.ToArray();
        }

        public bool Ok { get; set; }

        public string[] Erros { get; set; }

        public static RespostaApi Sucesso()
        {
            return new RespostaApi(true, new string[] { });
        }

        public static RespostaApi Falha(IEnumerable<string> errors)
        {
            return new RespostaApi(false, errors);
        }
    }
}
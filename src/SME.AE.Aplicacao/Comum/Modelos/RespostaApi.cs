using System.Collections.Generic;
using System.Linq;
namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class RespostaApi
    {
        internal RespostaApi() { }
        internal RespostaApi(bool ok, IEnumerable<string> erros, object data)
        {
            Ok = ok;
            Erros = erros.ToArray();
            Data = data;
        }
        public bool Ok { get; set; }
        public string[] Erros { get; set; }
        public object Data { get; set; }

        public static RespostaApi Sucesso(object data = null)
        {
            return new RespostaApi(true, new string[] { }, data);
        }
        public static RespostaApi Falha(IEnumerable<string> errors)
        {
            return new RespostaApi(false, errors, null);
        }
    }
}
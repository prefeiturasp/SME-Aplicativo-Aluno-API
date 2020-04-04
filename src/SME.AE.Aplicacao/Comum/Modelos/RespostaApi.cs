using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RespostaApi
    {
        internal RespostaApi(bool ok, IEnumerable<string> erros, object data)
        {
            Ok = ok;
            Erros = erros.ToArray();
            Data = data;
        }

        internal RespostaApi(bool ok, IList<ValidationFailure> erros)
        {
            Ok = ok;
            Erros = erros.Select(x => x.ErrorMessage).ToArray();
            Data = null;
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

        public static RespostaApi Falha(IList<ValidationFailure> erros)
        {
            return new RespostaApi(false, erros);
        }
    }
}

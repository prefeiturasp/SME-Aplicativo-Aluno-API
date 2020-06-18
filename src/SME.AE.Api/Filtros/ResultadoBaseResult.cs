using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Api.Filtros
{
    public class ResultadoBaseResult : ObjectResult
    {
        public ResultadoBaseResult(string mensagem)
            : base(RetornaBaseModel(mensagem))
        {
            StatusCode = 400;
        }

        public ResultadoBaseResult(RespostaApi retornoBaseDto) : base(retornoBaseDto)
        {
            StatusCode = 400;
        }

        public ResultadoBaseResult(string mensagem, int statusCode)
            : base(RetornaBaseModel(mensagem))
        {
            StatusCode = statusCode;
        }

        public static RespostaApi RetornaBaseModel(string mensagem)
        {
            return RespostaApi.Falha(mensagem);
        }
    }
}

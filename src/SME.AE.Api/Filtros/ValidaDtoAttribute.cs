using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Linq;

namespace SME.AE.Api.Filtros
{
    public class ValidaDtoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new FalhaValidacaoResult(context.ModelState);
            }
        }

        public class FalhaValidacaoResult : ObjectResult
        {
            public FalhaValidacaoResult(ModelStateDictionary modelState) : base(RetornaBaseModel(modelState))
            {
                StatusCode = 400;
            }

            public static RespostaApi RetornaBaseModel(ModelStateDictionary modelState)
            {
                return RespostaApi.Falha(modelState.Keys
                       .SelectMany(key => modelState[key].Errors.Select(x => new string(x.ErrorMessage)))
                       .ToArray());
                ;
            }

        }
    }
}

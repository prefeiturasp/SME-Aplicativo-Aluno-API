using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterComunicadosAnoAtualQuery : IRequest<IEnumerable<ComunicadoSgpDto>>
    {
        public ObterComunicadosAnoAtualQuery()
        {}
    }
}

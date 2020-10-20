using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotalUsuariosComAcessoIncompletoUseCase : IObterTotalUsuariosComAcessoIncompletoUseCase
    {
        private readonly IMediator mediator;

        public ObterTotalUsuariosComAcessoIncompletoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Executar(string codigoDre, string codigoUe)
        {
            var cpfsDeResponsaveis = new List<string>();
            if (!String.IsNullOrEmpty(codigoDre) || !String.IsNullOrEmpty(codigoUe))
            {
                var cpfsDaAbrangencia = await mediator.Send(new ObterCpfsDeResponsaveisPorDreEUeQuery(codigoDre, codigoUe));
                cpfsDeResponsaveis = ConverterCpfsParaLista(cpfsDaAbrangencia);
            }

            return await mediator.Send(new ObterTotalUsuariosComAcessoIncompletoQuery(cpfsDeResponsaveis));
        }

        private List<string> ConverterCpfsParaLista(IEnumerable<CpfResponsavelAlunoEol> cpfsResponsaveis) {
            return (cpfsResponsaveis.Select(item => item.CpfResponsavel)).ToList();
        }
    }
}

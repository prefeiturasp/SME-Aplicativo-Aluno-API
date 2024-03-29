﻿using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotalUsuariosValidosUseCase : IObterTotalUsuariosValidosUseCase
    {
        private readonly IMediator mediator;

        public ObterTotalUsuariosValidosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Executar(string codigoDre, string codigoUe)
        {
            var cpfsDeResponsaveis = new List<string>();
            if (!String.IsNullOrEmpty(codigoDre) || !String.IsNullOrEmpty(codigoUe))
            {
                var cpfsDaAbrangencia = await mediator.Send(new ObterResponsaveisPorDreEUeQuery(codigoDre, codigoUe));
                cpfsDeResponsaveis = ConverterCpfsParaLista(cpfsDaAbrangencia);
            }
            return await mediator.Send(new ObterTotalUsuariosValidosQuery(cpfsDeResponsaveis));
        }
        private List<string> ConverterCpfsParaLista(IEnumerable<ResponsavelAlunoEOLDto> cpfsResponsaveis)
        {
            return (cpfsResponsaveis.Select(item => item.CpfResponsavel.ToString()).Distinct()).ToList();
        }
    }
}

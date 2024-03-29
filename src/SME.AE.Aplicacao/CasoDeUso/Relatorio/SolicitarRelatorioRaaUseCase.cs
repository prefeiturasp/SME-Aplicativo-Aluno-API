﻿using MediatR;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class SolicitarRelatorioRaaUseCase : ISolicitarRelatorioRaaUseCase
    {
        private readonly IMediator mediator;

        public SolicitarRelatorioRaaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(SolicitarRelatorioRaaDto filtro)
        {
            return await mediator.Send(new SolicitarRelatorioRaaQuery(filtro.DreCodigo, filtro.UeCodigo, filtro.Semestre,filtro.TurmaCodigo,filtro.AnoLetivo,filtro.ModalidadeCodigo,filtro.AlunoCodigo));;
        }
    }
}

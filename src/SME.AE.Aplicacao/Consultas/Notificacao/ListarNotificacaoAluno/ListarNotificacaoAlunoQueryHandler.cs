﻿using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class ListarNotificacaoAlunoQueryHandler : IRequestHandler<ListarNotificacaoAlunoQuery, IEnumerable<NotificacaoResposta>>
    {
        private readonly INotificacaoRepositorio notificacaoRepository;

        public ListarNotificacaoAlunoQueryHandler(INotificacaoRepositorio notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Handle(ListarNotificacaoAlunoQuery request, CancellationToken cancellationToken)
        {
            var retorno = await notificacaoRepository.ListarNotificacoes(request.Modalidades, request.TiposEscolas, request.CodigoUE, request.CodigoDRE, request.CodigoTurma, request.CodigoAluno, request.CodigoUsuario, request.SerieResumida);

            if (retorno != null || retorno.Any())
                return retorno;
            else return default;
        }
    }
}

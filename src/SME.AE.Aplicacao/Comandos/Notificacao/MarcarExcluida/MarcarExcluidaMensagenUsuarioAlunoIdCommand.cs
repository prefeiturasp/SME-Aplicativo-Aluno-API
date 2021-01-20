using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class MarcarExcluidaMensagenUsuarioAlunoIdCommand : IRequest<bool>
    {
        public long NotificacaoId { get; set; }
        public long CodigoAluno { get; set; }
        public string Cpf { get; set; }
    }

    public class MarcarExcluidaMensagenUsuarioAlunoIdCommandHandler : IRequestHandler<MarcarExcluidaMensagenUsuarioAlunoIdCommand, bool>
    {
        private readonly IUsuarioNotificacaoRepositorio usuarioNotificacaoRepositorio;
        private readonly IUsuarioRepository usuarioRepository;

        public MarcarExcluidaMensagenUsuarioAlunoIdCommandHandler(IUsuarioNotificacaoRepositorio usuarioNotificacaoRepositorio, IUsuarioRepository usuarioRepository)
        {
            this.usuarioNotificacaoRepositorio = usuarioNotificacaoRepositorio ?? throw new ArgumentNullException(nameof(usuarioNotificacaoRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }


        public async Task<bool> Handle(MarcarExcluidaMensagenUsuarioAlunoIdCommand request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf) ?? throw new ArgumentException($"CPF não encontrado {request.Cpf}");

            var usuarioNotificacao = await usuarioNotificacaoRepositorio.ObterPorUsuarioAlunoNotificacao(usuario.Id, request.CodigoAluno, request.NotificacaoId);
            if(usuarioNotificacao == null)
            {
                usuarioNotificacao = new UsuarioNotificacao
                {
                    UsuarioId = usuario.Id,
                    UsuarioCpf = usuario.Cpf,
                    CriadoPor = usuario.Cpf,
                    NotificacaoId = request.NotificacaoId,
                    MensagemExcluida = true,
                    MensagemVisualizada = true
                };
                return await usuarioNotificacaoRepositorio.Criar(usuarioNotificacao);
            } else
            {
                usuarioNotificacao.MensagemExcluida = true;
                return await usuarioNotificacaoRepositorio.Atualizar(usuarioNotificacao);
            }
        }
    }
}
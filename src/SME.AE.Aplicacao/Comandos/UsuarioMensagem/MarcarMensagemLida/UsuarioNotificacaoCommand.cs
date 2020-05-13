﻿using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida
{
    public class UsuarioNotificacaoCommand : IRequest<bool>
    {
        public UsuarioNotificacaoCommand(long idMensagem, long idUsuario)
        {
            IdMensagem = idMensagem;
            IdUsuario = idUsuario;
        }
        private long IdMensagem { get; set; }
        private long IdUsuario { get; set; }

        public class UsuarioMensagemCommandHandler : IRequestHandler<UsuarioNotificacaoCommand, bool>
        {
            private readonly IUsuarioNotificacaoRepositorio _repository;

            public UsuarioMensagemCommandHandler(IUsuarioNotificacaoRepositorio repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(UsuarioNotificacaoCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    var mensagem = await _repository.Selecionar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem });
                    if (mensagem == null)
                        return await _repository.Criar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem });
                    return await _repository.RemoverPorId(mensagem.Id);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}

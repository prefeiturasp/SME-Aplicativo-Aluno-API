using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida
{
    public class UsuarioNotificacaoCommand : IRequest<bool>
    {
        public UsuarioNotificacaoCommand(UsuarioNotificacao usuarioNotificacao)
        {
            UsuarioNotificacao = usuarioNotificacao;
        }
        private UsuarioNotificacao UsuarioNotificacao { get; set; }
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
                    // atualizar tabela com dados 
                    // ver se existe 
                    // se existir Atualizar
                    // Se nao criar
                    // Retornar Notificacao com visualizacao true or false

                    var mensagem = await _repository.Selecionar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem });
                    if (mensagem == null)
                        return await _repository.Criar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem });
                    //retorna false 
                    return !await _repository.RemoverPorId(mensagem.Id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}

using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida
{
    public class UsuarioNotificacaoCommand : IRequest<NotificacaoUsuarioLeitura>
    {
        public UsuarioNotificacaoCommand(long idMensagem, long idUsuario, string ueId, string dreId)
        {
            IdMensagem = idMensagem;
            IdUsuario = idUsuario;
            UeId = ueId;
            DreId = dreId;
        }
        private long IdMensagem { get; set; }
        private long IdUsuario { get; set; }
        private string UeId { get; set; }
        private string DreId { get; set; }
        private bool MensagemLida { get; set; }

        public class UsuarioMensagemCommandHandler : IRequestHandler<UsuarioNotificacaoCommand, NotificacaoUsuarioLeitura>
        {
            private readonly IUsuarioNotificacaoRepositorio _repository;
            private readonly INotificacaoRepository _notificacaoRepository;


            public UsuarioMensagemCommandHandler(IUsuarioNotificacaoRepositorio repository, INotificacaoRepository notificacaoRepository)
            {
                _repository = repository;
                _notificacaoRepository = notificacaoRepository;
            }

            public async Task<NotificacaoUsuarioLeitura> Handle(UsuarioNotificacaoCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    Dominio.Entidades.UsuarioNotificacao usuarioNotificacao;
                    usuarioNotificacao = await BuscaUsuarioNotificacaoId(request);
                    
                    if (usuarioNotificacao == null)
                    {
                        request.MensagemLida = true;
                        await _repository.Criar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem, UeId = request.UeId, DreId = request.DreId , MensagemLida = request.MensagemLida }); ;
                        usuarioNotificacao = await BuscaUsuarioNotificacaoId(request);

                    }

                    else
                    {
                        usuarioNotificacao.MensagemLida = !usuarioNotificacao.MensagemLida;
                    }


                    var notificacaoLeitura = new NotificacaoUsuarioLeitura();
                    notificacaoLeitura.Notificaco = await _notificacaoRepository.ObterPorId(request.IdMensagem);
                    notificacaoLeitura.MensagemLida = usuarioNotificacao.MensagemLida;
                    return notificacaoLeitura;


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                async Task<Dominio.Entidades.UsuarioNotificacao> BuscaUsuarioNotificacaoId(UsuarioNotificacaoCommand request)
                {
                    return await _repository.Selecionar(new Dominio.Entidades.UsuarioNotificacao { UsuarioId = request.IdUsuario, NotificacaoId = request.IdMensagem ,});
                }
            }
        }
    }
}

using MediatR;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IMediator mediator;

        public MarcarExcluidaMensagenUsuarioAlunoIdCommandHandler(IUsuarioNotificacaoRepositorio usuarioNotificacaoRepositorio,
                                                                  IUsuarioRepository usuarioRepository,
                                                                  IMediator mediator)
        {
            this.usuarioNotificacaoRepositorio = usuarioNotificacaoRepositorio ?? throw new ArgumentNullException(nameof(usuarioNotificacaoRepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(MarcarExcluidaMensagenUsuarioAlunoIdCommand request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository
                .ObterUsuarioNaoExcluidoPorCpf(request.Cpf) ?? throw new ArgumentException($"CPF não encontrado {request.Cpf}");

            var usuarioNotificacao = await usuarioNotificacaoRepositorio
                .ObterPorUsuarioAlunoNotificacao(usuario.Id, request.CodigoAluno, request.NotificacaoId);

            if (usuarioNotificacao == null)
            {
                var resposta = await mediator.Send(new DadosAlunoCommand(request.Cpf));

                if (resposta.Data == null)
                    throw new NegocioException("Não foi possivel obter os alunos por escola");

                var listaEscolas = (IEnumerable<ListaEscola>)resposta.Data;

                var aluno = listaEscolas.FirstOrDefault()?.Alunos.FirstOrDefault();

                if (aluno == null)
                    throw new NegocioException($"Não encontrado usuário com o CPF {request.Cpf}");

                usuarioNotificacao = new UsuarioNotificacao
                {
                    CodigoEolAluno = aluno.CodigoEol,
                    UsuarioId = usuario.Id,
                    UsuarioCpf = usuario.Cpf,
                    CriadoPor = usuario.Cpf,
                    NotificacaoId = request.NotificacaoId,
                    MensagemExcluida = true,
                    MensagemVisualizada = true
                };

                return await usuarioNotificacaoRepositorio
                    .Criar(usuarioNotificacao);
            }
            else
            {
                usuarioNotificacao.MensagemExcluida = true;

                return await usuarioNotificacaoRepositorio
                    .Atualizar(usuarioNotificacao);
            }
        }
    }
}
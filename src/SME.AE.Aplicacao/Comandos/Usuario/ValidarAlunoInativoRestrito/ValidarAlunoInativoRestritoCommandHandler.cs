﻿using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito
{
    public class ValidarAlunoInativoRestritoCommandHandler : IRequestHandler<ValidarAlunoInativoRestritoCommand>
    {
        private readonly IMediator mediator;
        private readonly IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio;
        private readonly IUsuarioRepository usuarioRepository;

        public ValidarAlunoInativoRestritoCommandHandler(IMediator autenticamediatorcaoService, IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio, IUsuarioRepository usuarioRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.usuarioCoreSSORepositorio = usuarioCoreSSORepositorio ?? throw new ArgumentNullException(nameof(usuarioCoreSSORepositorio));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }


        public async Task<Unit> Handle(ValidarAlunoInativoRestritoCommand request, CancellationToken cancellationToken)
        {
            var usuarioAlunos = await mediator.Send(new ObterDadosResponsavelQuery(request.UsuarioCoreSSO.Cpf));

            if (usuarioAlunos is null || !usuarioAlunos.Any())
                await NenhumAlunoAtivo(request);

            if (usuarioAlunos.Any(x => x.TipoSigilo == (int)AlunoTipoSigilo.Restricao))
                await ResponsavelComRestricao(request);

            return default;
        }

        private async Task ResponsavelComRestricao(ValidarAlunoInativoRestritoCommand request)
        {
            await InativarUsuario(request);
            throw new NegocioException("Este CPF não existe na base do Escola Aqui. qualquer dúvida procure a unidade escolar.");
        }

        private async Task NenhumAlunoAtivo(ValidarAlunoInativoRestritoCommand request)
        {
            await InativarUsuario(request);
            throw new NegocioException("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.");
        }

        private async Task InativarUsuario(ValidarAlunoInativoRestritoCommand request)
        {
            await usuarioRepository.ExcluirUsuario(request.UsuarioCoreSSO.Cpf);

            await usuarioCoreSSORepositorio.AlterarStatusUsuario(request.UsuarioCoreSSO.UsuId, Comum.Enumeradores.StatusUsuarioCoreSSO.Inativo);
        }
    }
}

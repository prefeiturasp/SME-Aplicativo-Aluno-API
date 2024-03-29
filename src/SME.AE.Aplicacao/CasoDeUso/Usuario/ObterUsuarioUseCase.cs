﻿using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using SME.AE.Comum.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterUsuarioUseCase : IObterUsuarioUseCase
    {
        private readonly IMediator mediator;

        public ObterUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UsuarioDto> Executar(string codigoDre, long codigoUe, string cpf)
        {
            if (ValidacaoCpf.Valida(cpf) == false)
                throw new NegocioException($"CPF inválido!");

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(cpf));

            var alunos = await mediator.Send(new ObterDadosAlunosQuery(cpf, null, codigoDre, codigoUe == 0 ? "" : codigoUe.ToString()));
            if (alunos == null || !alunos.Any())
                throw new NegocioException("Este CPF não consta como responsável de um estudante ativo nesta Unidade Escolar.");

            var usuarioApp = await mediator.Send(new ObterUsuarioNaoExcluidoPorCpfQuery(cpf));

            var usuarioEol = await mediator.Send(new ObterDadosResponsavelResumidoQuery(cpf));

            if (usuarioCoreSSO == null)
                throw new NegocioException($"Este CPF não consta como responsável de um estudante ativo nesta Unidade Escolar.");

            if (usuarioApp == null && usuarioCoreSSO != null)
                throw new NegocioException($"O usuário {Formatacao.FormatarCpf(cpf)} deverá informar a data de nascimento de um dos estudantes que é responsável no campo de senha!");

            return new UsuarioDto(usuarioApp.Cpf, usuarioEol.Nome);
        }
    }
}
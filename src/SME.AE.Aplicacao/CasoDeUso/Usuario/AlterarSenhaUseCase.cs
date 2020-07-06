﻿using MediatR;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Aplicacao.Consultas.VerificarSenha;
using SME.AE.Aplicacao.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class AlterarSenhaUseCase : IAlterarSenhaUseCase
    {
        private readonly IMediator mediator;

        public AlterarSenhaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(AlterarSenhaDto alterarSenhaDto)
        {
            await ValidarDto(alterarSenhaDto);

            var usuario = await ObterUsuarioCoreSSO(alterarSenhaDto);

            usuario.Alterarsenha(alterarSenhaDto.Senha);

            await Validar5UltimasSenhas(usuario);

            await AlterarSenhaUsuarioCoreSSO(usuario);

            await IncluirSenhaHistorico(usuario);

            return RespostaApi.Sucesso();
        }

        private async Task Validar5UltimasSenhas(RetornoUsuarioCoreSSO usuario)
        {
            var validarUltimasSenhas = new VerificarUltimasSenhasQuery(usuario.UsuId, usuario.Senha);

            var resultado = await mediator.Send(validarUltimasSenhas);

            if (!resultado)
                throw new NegocioException("A sua nova senha deve ser diferente das últimas 5 senhas utilizadas.");
        }

        private async Task IncluirSenhaHistorico(RetornoUsuarioCoreSSO usuario)
        {
            var incluirSenhaHistorico = new AdicionarSenhaHistoricoCommand(usuario.UsuId, usuario.Senha);

            await mediator.Send(incluirSenhaHistorico);
        }

        private async Task AlterarSenhaUsuarioCoreSSO(RetornoUsuarioCoreSSO usuario)
        {
            var alterarSenhaUsuarioCore = new AlterarSenhaUsuarioCoreSSOCommand(usuario.UsuId, usuario.Senha);

            await mediator.Send(alterarSenhaUsuarioCore);
        }

        private async Task<RetornoUsuarioCoreSSO> ObterUsuarioCoreSSO(AlterarSenhaDto alterarSenhaDto)
        {
            var query = new ObterUsuarioCoreSSOQuery(alterarSenhaDto.CPF);   

            return await mediator.Send(query) ?? throw new NegocioException($"Usuário com o CPF '{alterarSenhaDto.CPF}' não encontrado");
        }

        private static async Task ValidarDto(AlterarSenhaDto alterarSenhaDto)
        {
            var validator = new AlterarSenhaValidator();

            var result = await validator.ValidateAsync(alterarSenhaDto);

            if (!result.IsValid)
                throw new ValidacaoException(result.Errors);
        }
    }
}

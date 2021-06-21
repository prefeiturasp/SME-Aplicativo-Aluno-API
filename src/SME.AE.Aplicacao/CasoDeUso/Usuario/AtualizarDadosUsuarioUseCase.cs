﻿using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarDadosUsuarioUseCase : IAtualizarDadosUsuarioUseCase
    {
        private readonly IMediator mediator;

        public AtualizarDadosUsuarioUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(AtualizarDadosUsuarioDto usuarioDto)
        {
            var usuarioApp = await mediator.Send(new ObterUsuarioQuery(usuarioDto.Id));

            bool podePersistirTexto = await PodePersistirTexto(usuarioDto);

            if (!podePersistirTexto)
                return RespostaApi.Falha("Conteúdo inadequado nos campos de cadastro, por favor revise e tente novamente.");

            var usuarioEol = await mediator.Send(new ObterDadosResumidosReponsavelPorCpfQuery(usuarioApp.Cpf));

            if (usuarioEol == null)
                return RespostaApi.Falha("Usuário não encontrado!");

            await AtualizaUsuario(usuarioApp, usuarioDto);            

            return RespostaApi.Sucesso();
        }

        private async Task AtualizaUsuario(Usuario usuarioApp, AtualizarDadosUsuarioDto usuarioDto)
        {
            usuarioApp.AtualizarAuditoria();
            await mediator.Send(new SalvarUsuarioCommand(usuarioApp));

            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralEol, usuarioDto, Guid.NewGuid()));
            await mediator.Send(new PublicarFilaAeCommand(RotasRabbitAe.RotaAtualizacaoCadastralProdam, usuarioDto, Guid.NewGuid()));
        }

        private async Task<bool> PodePersistirTexto(AtualizarDadosUsuarioDto usuarioDto)
        {
            var podePersistir = await mediator.Send(new VerificaPalavraProibidaPodePersistirCommand(usuarioDto.TextoParaVerificarPersistencia()));
            return podePersistir;
        }
    }
}

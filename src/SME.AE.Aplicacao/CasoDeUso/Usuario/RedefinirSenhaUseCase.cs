using MediatR;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao;
using SME.AE.Aplicacao.Consultas.VerificarSenha;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class RedefinirSenhaUseCase : IRedefinirSenhaUseCase
    {
        private readonly IMediator mediator;

        public RedefinirSenhaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(RedefinirSenhaDto dto)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorTokenRedefinicaoQuery(dto.Token));

            usuario.ValidarTokenRedefinicao(dto.Token);

            var usuarioCore = await mediator.Send(new ObterUsuarioCoreSSOQuery(usuario.Cpf));

            if (usuarioCore == null)
                throw new NegocioException("Usuário não encontrado no CoreSSO");

            usuarioCore.AlterarSenha(dto.Senha);

            await Validar5UltimasSenhas(usuarioCore);

            await AlterarSenhaUsuarioCoreSSO(usuarioCore);

            await IncluirSenhaHistorico(usuarioCore);

            usuario.FinalizarRedefinicaoSenha();

            await mediator.Send(new SalvarUsuarioCommand(usuario));

            return await AutenticarUsuario(dto, usuario);
        }

        private async Task<RespostaApi> AutenticarUsuario(RedefinirSenhaDto dto, Dominio.Entidades.Usuario usuario)
        {
            var autenticacao = await mediator.Send(new AutenticarUsuarioCommand(usuario.Cpf, dto.Senha));

            if (!autenticacao.Ok)
                throw new NegocioException(string.Join(',', autenticacao.Erros));

            var token = await mediator.Send(new CriarTokenCommand(usuario.Cpf));

            await mediator.Send(new UsuarioDispositivoCommand(usuario.Cpf, dto.DispositivoId));

            ((RespostaAutenticar)autenticacao.Data).Token = token;

            return autenticacao;
        }

        private async Task Validar5UltimasSenhas(RetornoUsuarioCoreSSO usuario)
        {
            var validarUltimasSenhas = new VerificarUltimasSenhasQuery(usuario.UsuId, usuario.Senha);

            var resultado = await mediator.Send(validarUltimasSenhas);

            if (resultado)
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
    }
}

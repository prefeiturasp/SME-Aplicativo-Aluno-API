using MediatR;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class PrimeiroAcessoUseCase : IPrimeiroAcessoUseCase
    {
        private readonly IMediator mediator;

        public PrimeiroAcessoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(NovaSenhaDto novaSenhaDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery() { Id = novaSenhaDto.Id });

            if (usuario == null)
                throw new NegocioException("Usuário não encontrado");

            if (!usuario.PrimeiroAcesso)
                throw new NegocioException("Somente é possivel utilizar essa função quando for o primeiro acesso do usuário");

            var usuarioEol = await mediator.Send(new ObterDadosResumidosReponsavelPorCpfQuery(usuario.Cpf));

            if (usuarioEol == null)
                throw new NegocioException("Responsável não encontrado");

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(usuario.Cpf));

            await CriarUsuarioOuAssociarGrupo(novaSenhaDto, usuario, usuarioCoreSSO, usuarioEol.Nome);

            var atualizarPrimeiroAcesso = MapearAtualizarPrimeiroAcessoCommand(usuario);

            await mediator.Send(atualizarPrimeiroAcesso);

            await IncluirSenhaHistorico(usuario.Cpf);

            return RespostaApi.Sucesso();
        }

        private async Task IncluirSenhaHistorico(string cpf)
        {
            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(cpf));

            var incluirSenhaHistorico = new AdicionarSenhaHistoricoCommand(usuarioCoreSSO.UsuId, usuarioCoreSSO.Senha);

            await mediator.Send(incluirSenhaHistorico);
        }

        private async Task CriarUsuarioOuAssociarGrupo(NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario, RetornoUsuarioCoreSSO usuarioCoreSSO, string nomeUsuario)
        {
            if (usuarioCoreSSO != null)
                await AssociarGrupoEAlterarSenha(novaSenhaDto, usuarioCoreSSO);
            else
                await CriarUsuario(novaSenhaDto, usuario, nomeUsuario);
        }

        private async Task AssociarGrupoEAlterarSenha(NovaSenhaDto novaSenhaDto, RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            usuarioCoreSSO.AlterarSenha(novaSenhaDto.NovaSenha);

            await mediator.Send(new AssociarGrupoUsuarioCommand(usuarioCoreSSO));

            await mediator.Send(new AlterarSenhaUsuarioCoreSSOCommand(usuarioCoreSSO.UsuId, usuarioCoreSSO.Senha));
        }

        private async Task CriarUsuario(NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario, string nomeUsuario)
        {
            var comandoCriaUsuario = MapearCriarUsuarioCoreSSOCommand(novaSenhaDto, usuario, nomeUsuario);

            await mediator.Send(comandoCriaUsuario);
        }

        private AtualizarPrimeiroAcessoCommand MapearAtualizarPrimeiroAcessoCommand(Dominio.Entidades.Usuario usuario)
        {
            return new AtualizarPrimeiroAcessoCommand
            {
                Id = usuario.Id,
                PrimeiroAcesso = false
            };
        }

        private CriarUsuarioCoreSSOCommand MapearCriarUsuarioCoreSSOCommand(NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario, string nomeUsuario)
        {
            return new CriarUsuarioCoreSSOCommand()
            {
                Usuario = new UsuarioCoreSSODto
                {
                    Cpf = usuario.Cpf,
                    Nome = nomeUsuario,
                    Senha = novaSenhaDto.NovaSenha,
                }
            };
        }

    }
}

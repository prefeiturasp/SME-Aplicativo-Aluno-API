using MediatR;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Extensoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class PrimeiroAcessoUseCase : ICriarUsuarioPrimeiroAcessoUseCase
    {
        private readonly IMediator mediator;

        public CriarUsuarioPrimeiroAcessoUseCase(IMediator mediator)
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

            var usuarioCoreSSO = await mediator.Send(new ObterUsuarioCoreSSOQuery(usuario.Cpf));

            await CriarUsuarioOuAssociarGrupo(mediator, novaSenhaDto, usuario, usuarioCoreSSO);

            var atualizarPrimeiroAcesso = MapearAtualizarPrimeiroAcessoCommand(usuario);

            await mediator.Send(atualizarPrimeiroAcesso);

            return new RespostaApi
            {
                Ok = true
            };
        }

        private async Task CriarUsuarioOuAssociarGrupo(IMediator mediator, NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario, RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            if (usuarioCoreSSO != null)
                await AssociarGrupoEAlterarSenha(mediator, novaSenhaDto, usuarioCoreSSO);
            else
                await CriarUsuario(mediator, novaSenhaDto, usuario);
        }

        private async Task AssociarGrupoEAlterarSenha(IMediator mediator, NovaSenhaDto novaSenhaDto, RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            await mediator.Send(new AssociarGrupoUsuarioCommand(usuarioCoreSSO));

            await mediator.Send(new AlterarSenhaUsuarioCoreSSOCommand(usuarioCoreSSO.UsuId, Criptografia.CriptografarSenha(novaSenhaDto.NovaSenha, usuarioCoreSSO.TipoCriptografia)));
        }

        private async Task CriarUsuario(IMediator mediator, NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario)
        {
            var comandoCriaUsuario = MapearCriarUsuarioCoreSSOCommand(novaSenhaDto, usuario);

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

        public void Executar<TValue>(Func<TValue> isAny)
        {
            throw new NotImplementedException();
        }

        private CriarUsuarioCoreSSOCommand MapearCriarUsuarioCoreSSOCommand(NovaSenhaDto novaSenhaDto, Dominio.Entidades.Usuario usuario)
        {
            return new CriarUsuarioCoreSSOCommand()
            {
                Usuario = new UsuarioCoreSSODto
                {
                    Cpf = usuario.Cpf,
                    Nome = usuario.Nome,
                    Senha = novaSenhaDto.NovaSenha,
                }
            };
        }
    }
}

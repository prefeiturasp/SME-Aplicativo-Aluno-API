using MediatR;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario
{
    public class CriarUsuarioPrimeiroAcessoUseCase : ICriarUsuarioPrimeiroAcessoUseCase
    {
        private readonly IMediator mediator;

        public CriarUsuarioPrimeiroAcessoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<RespostaApi> Executar(NovaSenhaDto novaSenhaDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioQuery() { Id = novaSenhaDto.Id });

            var comandoCriaUsuario = MapearCriarUsuarioCoreSSOCommand(novaSenhaDto, usuario);

            await mediator.Send(comandoCriaUsuario);

            var atualizarPrimeiroAcesso = MapearAtualizarPrimeiroAcessoCommand(usuario);

            await mediator.Send(atualizarPrimeiroAcesso);

            return new RespostaApi
            {
                Ok = true
            };
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

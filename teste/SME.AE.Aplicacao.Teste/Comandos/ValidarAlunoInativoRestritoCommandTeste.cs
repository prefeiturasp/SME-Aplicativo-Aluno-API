using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum.Excecoes;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class ValidarAlunoInativoRestritoCommandTeste
    {
        [Fact]
        public void Construtor_ComDependenciaNula_DeveLancarExcecao()
        {
            var mediator = new Mock<IMediator>();
            var coreSso = new Mock<IUsuarioCoreSSORepositorio>();
            var usuario = new Mock<IUsuarioRepository>();

            Assert.Throws<ArgumentNullException>(() => new ValidarAlunoInativoRestritoCommandHandler(null, coreSso.Object, usuario.Object));
            Assert.Throws<ArgumentNullException>(() => new ValidarAlunoInativoRestritoCommandHandler(mediator.Object, null, usuario.Object));
            Assert.Throws<ArgumentNullException>(() => new ValidarAlunoInativoRestritoCommandHandler(mediator.Object, coreSso.Object, null));
        }

        [Fact]
        public async Task Handle_ComAlunosAtivosSemRestricao_DeveRetornarSucessoESemInativar()
        {
            var mediator = new Mock<IMediator>();
            var coreSso = new Mock<IUsuarioCoreSSORepositorio>();
            var usuario = new Mock<IUsuarioRepository>();
            var retorno = CriarUsuarioCore();
            mediator.Setup(x => x.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([new DadosResponsavelAluno { TipoSigilo = 0 }]);
            var handler = new ValidarAlunoInativoRestritoCommandHandler(mediator.Object, coreSso.Object, usuario.Object);

            var resultado = await handler.Handle(new ValidarAlunoInativoRestritoCommand(retorno), CancellationToken.None);

            Assert.Equal(default, resultado);
            usuario.Verify(x => x.ExcluirUsuario(It.IsAny<string>()), Times.Never);
            coreSso.Verify(x => x.AlterarStatusUsuario(It.IsAny<Guid>(), It.IsAny<StatusUsuarioCoreSSO>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SemAlunos_DeveInativarUsuarioELancarExcecao()
        {
            var mediator = new Mock<IMediator>();
            var coreSso = new Mock<IUsuarioCoreSSORepositorio>();
            var usuario = new Mock<IUsuarioRepository>();
            var retorno = CriarUsuarioCore();
            mediator.Setup(x => x.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<DadosResponsavelAluno>());
            var handler = new ValidarAlunoInativoRestritoCommandHandler(mediator.Object, coreSso.Object, usuario.Object);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                handler.Handle(new ValidarAlunoInativoRestritoCommand(retorno), CancellationToken.None));

            Assert.Contains("não está relacionado", excecao.Message);
            usuario.Verify(x => x.ExcluirUsuario(retorno.Cpf), Times.Once);
            coreSso.Verify(x => x.AlterarStatusUsuario(retorno.UsuId, StatusUsuarioCoreSSO.Inativo), Times.Once);
        }

        [Fact]
        public async Task Handle_ComAlunoRestrito_DeveInativarUsuarioELancarExcecaoEspecifica()
        {
            var mediator = new Mock<IMediator>();
            var coreSso = new Mock<IUsuarioCoreSSORepositorio>();
            var usuario = new Mock<IUsuarioRepository>();
            var retorno = CriarUsuarioCore();
            mediator.Setup(x => x.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosResponsavelAluno>
                {
                    new DadosResponsavelAluno { TipoSigilo = (int)AlunoTipoSigilo.Restricao }
                });
            var handler = new ValidarAlunoInativoRestritoCommandHandler(mediator.Object, coreSso.Object, usuario.Object);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                handler.Handle(new ValidarAlunoInativoRestritoCommand(retorno), CancellationToken.None));

            Assert.Contains("não existe na base", excecao.Message);
            usuario.Verify(x => x.ExcluirUsuario(retorno.Cpf), Times.Once);
            coreSso.Verify(x => x.AlterarStatusUsuario(retorno.UsuId, StatusUsuarioCoreSSO.Inativo), Times.Once);
        }

        private static RetornoUsuarioCoreSSO CriarUsuarioCore()
        {
            return new RetornoUsuarioCoreSSO
            {
                UsuId = Guid.NewGuid(),
                Cpf = "12345678901"
            };
        }
    }
}

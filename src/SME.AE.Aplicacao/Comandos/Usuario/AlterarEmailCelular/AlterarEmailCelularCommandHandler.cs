using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular
{
    public class AlterarEmailCelularCommandHandler : IRequestHandler<AlterarEmailCelularCommand>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public AlterarEmailCelularCommandHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Unit> Handle(AlterarEmailCelularCommand request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository.ObterPorIdAsync(request.AlterarEmailCelularDto.Id);

            if (usuario is null)
                throw new Exception($"Não encontrado usuário com id {request.AlterarEmailCelularDto.Id}");

            if (request.AlterarEmailCelularDto.AlterarSenha)
                ValidarAlterarEmailTelefoneAlterarSenha(request, usuario);

            if (!string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Celular))
                usuario.Celular = request.AlterarEmailCelularDto.CelularBanco;

            if (!string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Email))
                usuario.Email = request.AlterarEmailCelularDto.Email;

            await usuarioRepository.AtualizarEmailTelefone(usuario.Id, usuario.Email, usuario.Celular);

            return default;
        }

        private static void ValidarAlterarEmailTelefoneAlterarSenha(AlterarEmailCelularCommand request, Dominio.Entidades.Usuario usuario)
        {
            var erros = new Dictionary<string, string[]>();

            if (!string.IsNullOrWhiteSpace(usuario.Email) && string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Email))
                erros.Add("Email", new string[] { "Não pode ser removido o Email, apenas inserido ou atualizado" });

            if (!string.IsNullOrWhiteSpace(usuario.Celular) && string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Celular))
                erros.Add("Celular", new string[] { "Não pode ser removido o Celular, apenas inserido ou atualizado" });

            if (erros.Any())
                throw new ValidacaoException(erros);
        }
    }
}

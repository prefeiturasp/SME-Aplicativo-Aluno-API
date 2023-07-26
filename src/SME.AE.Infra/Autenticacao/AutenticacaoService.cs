using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using System.Collections.Generic;
using MediatR;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Infra.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly UserManager<UsuarioAplicacao> _userManager;

        public AutenticacaoService(UserManager<UsuarioAplicacao> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> ObterNomeUsuarioAsync(string id)
        {
            var usuario = await _userManager.Users.FirstAsync(u => u.Id == id);
            return usuario.UserName;
        }

        public async Task<(RespostaApi resposta, string id)> CriarUsuarioAsync(string cpf, string senha)
        {
            var usuario = new UsuarioAplicacao
            {
                UserName = cpf
            };

            var result = await _userManager.CreateAsync(usuario, senha);
            return (result.ParaRespostaApi(), usuario.Id);
        }
    }
}
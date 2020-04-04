using SME.AE.Aplicacao.Comum.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Infra.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly UserManager<UsuarioAplicacao> _userManager;
        private readonly IAutenticacaoRepositorio _autenticaacoRepositorio;

        public AutenticacaoService(UserManager<UsuarioAplicacao> userManager, IAutenticacaoRepositorio autenticaacoRepositorio)
        {
            _userManager = userManager;
            _autenticaacoRepositorio = autenticaacoRepositorio;
        }


        public async Task<bool> ValidarUsuarioEol(string cpf, DateTime dataNascimentoAluno)
        {
            return await _autenticaacoRepositorio.ValidarUsuarioEol(cpf, dataNascimentoAluno);
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
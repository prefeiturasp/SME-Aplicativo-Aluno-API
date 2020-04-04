using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IAutenticacaoRepositorio
    {
        Task<bool> ValidarUsuarioEol(string cpf, DateTime dataNascimentoAluno);
    }
}

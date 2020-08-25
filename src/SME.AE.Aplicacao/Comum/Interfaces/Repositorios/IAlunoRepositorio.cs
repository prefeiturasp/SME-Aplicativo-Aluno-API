using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
  public  interface IAlunoRepositorio
    {
        Task<List<AlunoRespostaEol>> ObterDadosAlunos(string cpf);
        Task<List<AlunoRespostaEol>> ObterDadosAlunosCache(string cpf);
    }
}

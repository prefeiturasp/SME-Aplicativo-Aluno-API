using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
  public  interface ITesteRepositorio
    {
        Task<DateTime> ObterDataHoraBanco();
    }
}

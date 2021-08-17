﻿using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotificacaoTurmaRepositorio
    {
        public  Task<bool> RemoverPorNotificacaoId(long id);
        public Task<bool> RemoverPorNotificacoesIds(long[] id);
    }
}
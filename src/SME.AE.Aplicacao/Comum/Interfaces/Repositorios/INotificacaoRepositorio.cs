﻿using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotificacaoRepositorio : IBaseRepositorio<Notificacao>
    {
        public Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf);

        public Task Criar(Notificacao notificacao);

        public Task Atualizar(AtualizarNotificacaoDto notificacao);

        public Task<bool> Remover(Notificacao notificacao);

        Task<IEnumerable<NotificacaoTurma>> ObterTurmasPorNotificacao(long id);

        public Task InserirNotificacaoAluno(NotificacaoAluno notificacaoAluno);

        public Task InserirNotificacaoTurma(NotificacaoTurma notificacaoTurma);
        Task<IEnumerable<NotificacaoResposta>> ListarNotificacoes(string modalidades, string tiposEscolas, string codigoUe, string codigoDre, string codigoTurma, string codigoAluno, long usuarioId, string serieResumida, DateTime? ultimaAtualizacao = null);
        Task<NotificacaoResposta> NotificacaoPorId(long Id);
        Task<IEnumerable<NotificacaoSgpDto>> ListarNotificacoesNaoEnviadas();
        Task<IEnumerable<NotificacaoAlunoResposta>> ObterNotificacoesAlunoPorId(long id);
    }
}
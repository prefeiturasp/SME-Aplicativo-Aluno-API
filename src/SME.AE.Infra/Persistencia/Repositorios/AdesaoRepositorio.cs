﻿using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AdesaoRepositorio : BaseRepositorio<Adesao>, IAdesaoRepositorio
    {
        private readonly IServicoTelemetria servicoTelemetria;

        public AdesaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoAgrupadosPorDre()
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();

                var dadosAdesaoAgrupadosPorDreUeETurma = await servicoTelemetria.RegistrarComRetornoAsync<TotaisAdesaoResultado>(async () =>
                    await SqlMapper.QueryAsync<TotaisAdesaoResultado>(conexao, AdesaoConsultas.ObterDadosAdesaoPorDre), "query", "Query AE", AdesaoConsultas.ObterDadosAdesaoPorDre);

                conexao.Close();

                return dadosAdesaoAgrupadosPorDreUeETurma;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoSintetico(string codigoDre, string codigoUe)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoDre))
                    codigoDre = "";

                if (string.IsNullOrWhiteSpace(codigoUe))
                    codigoUe = "";

                using var conexao = InstanciarConexao();
                conexao.Open();

                var parametros = new { dre_codigo = codigoDre, ue_codigo = codigoUe };

                var dadosAdesaoAgrupadosPorDreUeETurma = await servicoTelemetria.RegistrarComRetornoAsync<TotaisAdesaoResultado>(
                    async () => await SqlMapper.QueryAsync<TotaisAdesaoResultado>(conexao, AdesaoConsultas.ObterDadosAdesao, parametros),
                    "query", "Query AE", AdesaoConsultas.ObterDadosAdesao, parametros.ToString());
                    
                conexao.Close();

                return dadosAdesaoAgrupadosPorDreUeETurma;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
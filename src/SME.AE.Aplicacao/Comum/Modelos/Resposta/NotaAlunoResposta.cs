﻿namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class NotaAlunoResposta
    {
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public int Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Nota { get; set; }
        public string NotaDescricao { get; set; }
        public string CorNotaAluno { get; set; }
        public string RecomendacoesFamilia { get; set; }
        public string RecomendacoesAluno { get; set; }
    }
}
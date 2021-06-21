using System;

namespace SME.AE.Aplicacao.Servicos
{
    public interface IServicoLog
    {
        void Registrar(Exception ex);

        void Registrar(string mensagem);

        void RegistrarDependenciaAppInsights(string tipoDependencia, string alvo, string mensagem, DateTimeOffset inicio, TimeSpan duracao, bool sucesso);
    }
}
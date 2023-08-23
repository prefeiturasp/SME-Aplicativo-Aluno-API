using System;

namespace SME.AE.Aplicacao.Servicos
{
    public interface IServicoLog
    {
        void Registrar(Exception ex);

        void Registrar(string mensagem);
    }
}
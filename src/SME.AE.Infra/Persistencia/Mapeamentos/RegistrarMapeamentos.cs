using Dapper.FluentMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public static class RegistrarMapeamentos
    {
        public static void Registrar()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new NotificacaoMap());
                config.AddMap(new UsuarioNotificacaoMap());
                config.AddMap(new UsuarioMap());
            });
        }
    }
}

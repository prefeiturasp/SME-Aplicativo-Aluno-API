﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Interfaces.Contextos
{
    public interface IAplicacaoDapperContext<T> where T : IDbConnection
    {
        IDbConnection Conexao { get; }
    }
}

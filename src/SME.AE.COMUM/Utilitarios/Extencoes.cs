﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.AE.Comum.Utilitarios
{
    public static class StringListaIds
    {
        public static IEnumerable<string> ToStringEnumerable(this string stringLista, char delimitador = ',')
        {
            if (!String.IsNullOrEmpty(stringLista))
                foreach (var stringItem in stringLista.Split(delimitador))
                    if (!string.IsNullOrWhiteSpace(stringItem))
                        yield return stringItem.Trim();
        }
        public static IEnumerable<long> ToLongEnumerable(this string stringLista)
        {
            foreach (var stringId in stringLista.ToStringEnumerable())
                if (Int64.TryParse(stringId, out var numero))
                    yield return numero;
        }
        public static IEnumerable<int> ToIntEnumerable(this string stringLista)
        {
            foreach (var stringId in stringLista.ToStringEnumerable())
                if (Int32.TryParse(stringId, out var numero))
                    yield return numero;
        }
    }
}

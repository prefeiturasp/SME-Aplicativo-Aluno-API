﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace SME.AE.Dominio.Entidades
{
    public class AceiteTermosDeUso : EntidadeBase
    {
        public long TermosDeUsoId { get; private set; }
        public string Usuario { get; private set; }
        public string Device { get; private set; }
        public string Ip { get; private set; }
        public DateTime DataHoraAceite { get; private set; }
        public double Versao { get; private set; }
        public byte[] Hash { get; private set; }

        public AceiteTermosDeUso(long termosDeUsoId, string usuario, string device, string ip, double versao)
        {
            TermosDeUsoId = termosDeUsoId;
            Usuario = usuario;
            Device = device;
            Ip = ip;
            DataHoraAceite = DateTime.Now;
            Versao = versao;
            Hash = GerarHash($"{usuario}{device}{ip}{versao}");
        }

        private byte[] GerarHash(string valor)
        {
            return new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(valor));
        }
    }
}
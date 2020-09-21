using MediatR;
using System;

namespace SME.AE.Aplicacao.Comandos.TermosDeUso
{
    public class RegistrarAceiteDosTermosDeUsoCommand : IRequest<bool>
    {
        public RegistrarAceiteDosTermosDeUsoCommand(long termoDeUsoId, string usuario, string device, string ip, double versao)
        {
            TermoDeUsoId = termoDeUsoId;
            Usuario = usuario;
            Device = device;
            Ip = ip;
            Versao = versao;
        }

        public long TermoDeUsoId { get; set; }
        public string Usuario { get; set; }
        public string Device { get; set; }
        public string Ip { get; set; }
        public double Versao { get; set; }
    }
}

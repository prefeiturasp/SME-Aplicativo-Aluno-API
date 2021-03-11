using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterStatusNotificacaoUsuarioQueryHandler : IRequestHandler<ObterStatusNotificacaoUsuarioQuery, IEnumerable<StatusNotificacaoUsuario>>
    {
        private readonly IDadosLeituraRepositorio dadosLeituraRepositorio;
        private readonly IUsuarioRepository usuarioRepositorio;
        private readonly IAlunoRepositorio alunoRepositorio;

        public ObterStatusNotificacaoUsuarioQueryHandler(IDadosLeituraRepositorio dadosLeituraRepositorio, 
                                                         IUsuarioRepository usuarioRepositorio, 
                                                         IAlunoRepositorio alunoRepositorio)
        {
            this.dadosLeituraRepositorio = dadosLeituraRepositorio ?? throw new ArgumentNullException(nameof(dadosLeituraRepositorio));
            this.usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            this.alunoRepositorio = alunoRepositorio ?? throw new ArgumentNullException(nameof(alunoRepositorio));
        }

        public async Task<IEnumerable<StatusNotificacaoUsuario>> Handle(ObterStatusNotificacaoUsuarioQuery request, CancellationToken cancellationToken)
        {
            var statusNotificacaoUsuario = new List<StatusNotificacaoUsuario>();

            var aluno = await alunoRepositorio.ObterDadosAlunoPorCodigo(request.CodigoAluno);

            Usuario usuario = null;
            if (aluno != null)
            {
                usuario = await usuarioRepositorio.ObterPorCpf(aluno.CpfResponsavel);
            }

            foreach (var notificacaoId in request.NotificoesId)
            {
                var dadosLeituraComunicados = await dadosLeituraRepositorio.ObterDadosLeituraAlunos(notificacaoId, request.CodigoAluno.ToString());

                if (dadosLeituraComunicados.Any())
                {
                    statusNotificacaoUsuario.Add(new StatusNotificacaoUsuario(notificacaoId, dadosLeituraComunicados.First().DataLeitura.Value, $"Lida - em {dadosLeituraComunicados.First().DataLeitura.Value.ToString("dd/MM/yyyy HH:mm")}"));
                }

                if (!dadosLeituraComunicados.Any() && usuario != null)
                {
                    statusNotificacaoUsuario.Add(new StatusNotificacaoUsuario(notificacaoId, null, $"Não lida"));
                }

                if (!dadosLeituraComunicados.Any() && usuario == null)
                {
                    statusNotificacaoUsuario.Add(new StatusNotificacaoUsuario(notificacaoId, null, $"Não tem APP"));
                }
            }
            return statusNotificacaoUsuario;
        }
    }
}

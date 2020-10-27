using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class DashboardAdesaoDto
    {
        public string dre_codigo { get; set; }
        public string dre_nome { get; set; }
        public string ue_codigo { get; set; }
        public string ue_nome { get; set; }
        public long codigo_turma { get; set; }
        public int usuarios_primeiro_acesso_incompleto { get; set; }
        public int usuarios_validos { get; set; }
        public int usuarios_cpf_invalidos { get; set; }
        public int usuarios_sem_app_instalado { get; set; }
    }
}

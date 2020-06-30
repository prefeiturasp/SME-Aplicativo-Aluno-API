using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Teste.Persistencia.Repositorios
{
    public class GrupoComunicadoRepositoryMock : IGrupoComunicadoRepository
    {
        public Task<IEnumerable<GrupoComunicado>> ObterPorIds(string ids)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<GrupoComunicado>> ObterTodos()
        {
            return Task.FromResult(new List<GrupoComunicado>
            {
                new GrupoComunicado
                {
                    Id = 1, 
                    Nome = "EMEBS", 
                    TipoEscolaId = "4",
                    TipoCicloId = null,
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                },
                new GrupoComunicado
                {
                    Id = 2, 
                    Nome = "CEI", 
                    TipoEscolaId = null,
                    TipoCicloId = "1,23",
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                },
                new GrupoComunicado
                {
                    Id = 3, 
                    Nome = "EMEI", 
                    TipoEscolaId = null,
                    TipoCicloId = "2,14",
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                },
                new GrupoComunicado
                {
                    Id = 1, 
                    Nome = "Fundamental", 
                    TipoEscolaId = null,
                    TipoCicloId = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17",
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                },
                new GrupoComunicado
                {
                    Id = 5, 
                    Nome = "Médio", 
                    TipoEscolaId = null,
                    TipoCicloId = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17",
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                },
                new GrupoComunicado
                {
                    Id = 6, 
                    Nome = "EJA", 
                    TipoEscolaId = null,
                    TipoCicloId = "2,14",
                    CriadoEm = DateTime.Parse("2020-04-08 22:16:51.027671"),
                    CriadoRf = "Carga Inicial",
                    CriadoPor = "Carga Inicial",
                    Excluido = false
                }
            }.AsEnumerable());
        }
    }
}
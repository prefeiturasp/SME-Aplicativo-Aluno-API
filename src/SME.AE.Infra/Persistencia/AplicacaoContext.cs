using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SME.AE.Aplicacao.Comum.Interfaces.Geral;
using SME.AE.Dominio.Comum;
using SME.AE.Infra.Autenticacao;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia
{
    public class AplicacaoContext : ApiAuthorizationDbContext<UsuarioAplicacao>, IAplicacaoContext
    {
        public AplicacaoContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<EntidadeAuditavel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // entry.Entity.CreatedBy = _currentUserService.IdUsuario;
                        entry.Entity.Created = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        // entry.Entity.LastModifiedBy = _currentUserService.IdUsuario;
                        entry.Entity.LastModified = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

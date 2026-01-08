using Forex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forex.Infrastructure.Persistence.DbContext
{
    public class ForexDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ForexDbContext(DbContextOptions<ForexDbContext> options)
            : base(options)
        {
        }

        public DbSet<PriceTick> PriceTicks => Set<PriceTick>();
        public DbSet<SubscriptionAudit> SubscriptionAudits => Set<SubscriptionAudit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ForexDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}

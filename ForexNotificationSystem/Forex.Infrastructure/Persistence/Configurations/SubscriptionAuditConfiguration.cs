using Forex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forex.Infrastructure.Persistence.Configurations
{
    public class SubscriptionAuditConfiguration : IEntityTypeConfiguration<SubscriptionAudit>
    {
        public void Configure(EntityTypeBuilder<SubscriptionAudit> builder)
        {
            builder.ToTable("subscription_audit");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Symbol)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Action)
                   .IsRequired();

            builder.Property(x => x.ActionAtUtc)
                   .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Symbol);
        }
    }
}

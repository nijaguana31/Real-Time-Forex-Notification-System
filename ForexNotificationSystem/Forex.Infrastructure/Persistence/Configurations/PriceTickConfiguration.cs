using Forex.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forex.Infrastructure.Persistence.Configurations
{
    public class PriceTickConfiguration : IEntityTypeConfiguration<PriceTick>
    {
        public void Configure(EntityTypeBuilder<PriceTick> builder)
        {
            builder.ToTable("price_tick");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Symbol)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Price)
                   .HasPrecision(18, 6);

            builder.Property(x => x.Bid)
                   .HasPrecision(18, 6);

            builder.Property(x => x.Ask)
                   .HasPrecision(18, 6);

            builder.Property(x => x.TimestampUtc)
                   .IsRequired();

            builder.HasIndex(x => x.Symbol);
            builder.HasIndex(x => x.TimestampUtc);
        }
    }
}

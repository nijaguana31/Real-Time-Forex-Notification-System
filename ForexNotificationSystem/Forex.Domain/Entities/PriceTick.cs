namespace Forex.Domain.Entities
{
    public class PriceTick
    {
        public Guid Id { get; set; }

        public string Symbol { get; set; } = default!;

        public decimal Price { get; set; }

        public decimal Bid { get; set; }

        public decimal Ask { get; set; }

        public DateTime TimestampUtc { get; set; }
    }
}

namespace Forex.Application.DTOs
{
    public class PriceTickDto
    {
        public string Symbol { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}

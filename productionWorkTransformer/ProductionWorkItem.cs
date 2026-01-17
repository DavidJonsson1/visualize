using System;

namespace productionWorkTransformer
{
    public class ProductionWorkItem
    {
        public int ID { get; set; }
        public string Product { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Km { get; set; }
        public double Amount { get; set; }

        public TimeSpan Duration => EndDate - StartDate;

        public ProductionWorkItem Clone()
        {
            return new ProductionWorkItem
            {
                ID = this.ID,
                Product = this.Product,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Km = this.Km,
                Amount = this.Amount
            };
        }
    }
}

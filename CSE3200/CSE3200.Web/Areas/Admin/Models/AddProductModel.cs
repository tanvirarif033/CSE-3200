namespace CSE3200.Web.Areas.Admin.Models
{
    public class AddProductModel
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; } = 1.0;
        public string Details { get; set; }
        public int Quantity { get; set; } = 0;
    }
}

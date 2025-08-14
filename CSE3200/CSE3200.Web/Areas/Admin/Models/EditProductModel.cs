namespace CSE3200.Web.Areas.Admin.Models
{
    public class EditProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public string Details { get; set; }
        public int Quantity { get; set; }
    }
}

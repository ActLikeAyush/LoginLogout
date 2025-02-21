namespace TestAPIadminPortal.Models.Entites
{
    public class Product
    {
        public Guid ProductId { get; set; }    
        public string ProductName { get; set; }                

        public string ProductDescription { get; set; }
        public string ProductPrice { get; set; }

        public string ProductImg { get; set; }

    }
}

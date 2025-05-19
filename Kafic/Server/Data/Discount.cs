namespace Server.Data
{
    public class Discount
    {
        public int Id { get; set; }
        public int CaffeId { get; set; }
        public virtual Caffe? Caffe { get; set; }
        public string? DeskNo { get; set; }
        public double DiscountPercentage { get; set; }
    }
}

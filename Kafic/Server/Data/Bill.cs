namespace Server.Data
{
    public class Bill
    {
        public int Id { get; set; }
        public int CaffeId { get; set; }
        public virtual Caffe? Caffe { get; set; }
        public DateTime Date { get; set; }
        public double Discount { get; set; }
        //Uracunat je popust u TotalPrice
        public double Price { get; set; }
        public virtual IList<OrderPaid> OrderPaids { get; set; } = new List<OrderPaid>();
    }
}

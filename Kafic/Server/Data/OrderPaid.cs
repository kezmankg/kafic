namespace Server.Data
{
    public class OrderPaid
    {
        public int Id { get; set; }
        public string? DeskNo { get; set; }
        public DateTime Date { get; set; }
        public virtual IList<OrderPaidArticle> OrderArticles { get; set; } = new List<OrderPaidArticle>();
        public string? ApplicationUserEmail { get; set; }
        public int BillId { get; set; }
        public virtual Bill? Bill { get; set; }
        //Uracunat je popust u TotalPrice
        public double TotalPrice { get; set; }
    }
}

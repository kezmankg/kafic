namespace Server.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int CaffeId { get; set; }
        public virtual Caffe? Caffe { get; set; }
        public string? DeskNo { get; set; }
        public DateTime Date { get; set; }
        public virtual IList<OrderArticle> OrderArticles { get; set; } = new List<OrderArticle>();
        public string? ApplicationUserEmail { get; set; }

        // billId imacemo klasu Order2 koja ce cuvati sve ordere koji su izvrseni i tu ce se cuvati billId
    }
}

namespace Server.Data
{
    public class OrderArticle
    {        
        public int ArticleId { get; set; }
        public virtual Article? Article { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public int Amount { get; set; }
        public double Discount { get; set; }
    }
}

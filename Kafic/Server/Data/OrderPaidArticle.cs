namespace Server.Data
{
    public class OrderPaidArticle
    {
        public int ArticleId { get; set; }
        public virtual Article? Article { get; set; }
        public int OrderPaidId { get; set; }
        public virtual OrderPaid? Order { get; set; }
        public int Amount { get; set; }
    }
}

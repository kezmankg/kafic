using Share.Models;

namespace Client.Contracts
{
    public interface IOrderRepository
    {
        public Task<bool> AddOrder(OrderModel group);
        public Task<IList<OrderModel>> GetAllOrder(string email, string deskno);
        public Task<bool> DeleteOrder(int id);
        public Task<bool> DeleteArticle(int idOrder, int idArticle);
        public Task<bool> PayOrder(string descNo, string userEmail);

    }
}

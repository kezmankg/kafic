using Share.Models;

namespace Client.Contracts
{
    public interface IOrderRepository
    {
        public Task<bool> AddOrder(OrderModel group);
    }
}

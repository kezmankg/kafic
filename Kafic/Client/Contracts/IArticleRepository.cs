using Share.Models;

namespace Client.Contracts
{
    public interface IArticleRepository
    {
        public Task<bool> AddGroup(GroupModel group);
        public Task<IList<GroupModel>> GetAllGroup(string email);
    }
}

using Share.Models;

namespace Client.Contracts
{
    public interface IArticleRepository
    {
        public Task<bool> AddGroup(GroupModel group);
        public Task<IList<GroupModel>> GetAllGroup(string email);
        public Task<GroupModel> GetGroupById(string id);
        public Task<bool> UpdateGroup(GroupModel model);
    }
}

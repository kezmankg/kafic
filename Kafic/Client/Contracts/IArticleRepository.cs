using Share.Models;

namespace Client.Contracts
{
    public interface IArticleRepository
    {
        public Task<bool> AddGroup(GroupModel group);
        public Task<IList<GroupModel>> GetAllGroup(string email);
        public Task<GroupModel> GetGroupById(string id);
        public Task<bool> UpdateGroup(GroupModel model);
        public Task<bool> AddSubGroup(SubgroupModel group);
        public Task<IList<GroupModel>> GetAllGroupWithSubgroup(string email);
        public Task<SubgroupModel> GetSubGroupById(string id);
        public Task<bool> SubUpdateGroup(SubgroupModel model);
        public Task<bool> AddArticle(ArticleModel group);
        public Task<IList<GroupModel>> GetAllArticles(string email);
        public Task<ArticleModel> GetArticleById(string id);
        public Task<bool> UpdateArtile(ArticleModel model);
    }
}

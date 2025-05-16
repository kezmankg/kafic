namespace Client.Static
{
    public static class Endpoints
    {
#if DEBUG
        public static string BaseUrl = "https://localhost:7001/";
#else
        public static string BaseUrl = "";
#endif
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register/";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";
        public static string GetCompanyEndpoint = $"{BaseUrl}api/users/getCompany/";
        public static string UpdateCompanyEndpoint = $"{BaseUrl}api/users/updateCompany/";
        public static string RegisterUserEndpoint = $"{BaseUrl}api/users/registerUser/";
        public static string GetAllUsersEndpoint = $"{BaseUrl}api/users/getAllUsers/";
        public static string DeleteUserEndpoint = $"{BaseUrl}api/users/deleteUser/";
        public static string GetUserByIdEndpoint = $"{BaseUrl}api/users/getUser/";
        public static string UpdateUserEndpoint = $"{BaseUrl}api/users/updateUser/";
        public static string UpdateUserPasswordEndpoint = $"{BaseUrl}api/users/updateUserPassword/";

        public static string AddGroupEndpoint = $"{BaseUrl}api/article/addGroup/";
        public static string GetAllGroupsEndpoint = $"{BaseUrl}api/article/getAllGroups/";
        public static string GetGroupByIdEndpoint = $"{BaseUrl}api/article/getGroup/";
        public static string UpdateGroupEndpoint = $"{BaseUrl}api/article/updateGroup/";
        public static string AddSubGroupEndpoint = $"{BaseUrl}api/article/addSubGroup/";
        public static string GetGroupByIdWithSubgroupEndpoint = $"{BaseUrl}api/article/getGroupWithSubgroup/";
        public static string GetSubGroupByIdEndpoint = $"{BaseUrl}api/article/getSubGroup/";
        public static string UpdateSubGroupEndpoint = $"{BaseUrl}api/article/updateSubGroup/";
        public static string AddArticleEndpoint = $"{BaseUrl}api/article/addArticle/";
        public static string GetAllArticlesEndpoint = $"{BaseUrl}api/article/getAllArticles/";
        public static string GetArticleByIdEndpoint = $"{BaseUrl}api/article/getArticle/";
        public static string UpdateArticleEndpoint = $"{BaseUrl}api/article/updateArticle/";
        public static string DeleteArticleEndpoint = $"{BaseUrl}api/article/deleteArticle/";
        public static string DeleteSubgroupEndpoint = $"{BaseUrl}api/article/deleteSubgroup/";
        public static string DeleteGroupEndpoint = $"{BaseUrl}api/article/deleteGroup/";

        public static string AddOrderEndpoint = $"{BaseUrl}api/order/addOrder/";
        public static string GetAllOrdersEndpoint = $"{BaseUrl}api/order/getAllOrders/";
        public static string DeleteOrderEndpoint = $"{BaseUrl}api/order/deleteOrder/";
        public static string DeleteOrderArticleEndpoint = $"{BaseUrl}api/order/deleteArticle/";
    }
}

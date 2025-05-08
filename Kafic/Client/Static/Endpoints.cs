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
    }
}

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
    }
}

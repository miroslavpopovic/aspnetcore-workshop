namespace TimeTracker.Client
{
    public static class Config
    {
        public const string ApiRootUrl = "https://localhost:44383/";
        public const string ApiResourceUrl = ApiRootUrl + "api/";
        public const string TokenUrl = ApiRootUrl + "get-token";
    }
}

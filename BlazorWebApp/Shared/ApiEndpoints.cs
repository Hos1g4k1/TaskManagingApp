namespace BlazorWebApp.Shared
{
    public static class ApiEndpoints
    {
        private const string BaseApi = "api";

        public static string Projects => $"{BaseApi}/Project";
        public static string Statuses => $"{BaseApi}/Status";
    }
}
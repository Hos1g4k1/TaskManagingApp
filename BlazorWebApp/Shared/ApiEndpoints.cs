namespace BlazorWebApp.Shared
{
    public static class ApiEndpoints
    {
        private const string BaseApi = "api";

        public static string Projects => $"{BaseApi}/Project";
        public static string Statuses => $"{BaseApi}/Status";
        public static string Tasks => $"{BaseApi}/Task";
        public static string Comments => $"{BaseApi}/Comment";

        public static string TasksByProject(long projectId) => $"{Tasks}/Project/{projectId}";
        public static string CommentsByTask(long taskId) => $"{Comments}/task/{taskId}";
    }
}
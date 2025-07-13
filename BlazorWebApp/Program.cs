using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWebApp;
using BlazorWebApp.Services;

namespace project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Configure HTTP client to point to the REST API
            builder.Services.AddScoped(sp =>
                new HttpClient
                {
                    BaseAddress = new Uri(
                    builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress)
                });

            // Register services
            builder.Services.AddScoped<HttpClientService>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<StatusService>();
            builder.Services.AddScoped<TaskService>();
            builder.Services.AddScoped<CommentService>();

            await builder.Build().RunAsync();
        }
    }
}
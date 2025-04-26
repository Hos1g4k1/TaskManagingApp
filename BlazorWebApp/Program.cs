using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWebApp;

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

namespace project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => 
                new HttpClient { BaseAddress = new Uri(
                    builder.HostEnvironment.BaseAddress) });
            // builder.Services.AddScoped<IActorService, ActorService>();
            // builder.Services.AddSingleton<IMessagingService, MessagingService>();

            await builder.Build().RunAsync();
        }
    }
}
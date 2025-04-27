using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Supabase;
using REST;
using REST.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Debug);


var supabaseConfiguration = builder.Configuration
    .GetSection("Supabase")
    .Get<SupabaseConfiguration>();

builder.Services.AddScoped<Supabase.Client>(
    provider => new Supabase.Client(supabaseConfiguration.Url, supabaseConfiguration.Key));

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

var app = builder.Build();


app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

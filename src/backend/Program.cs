using System.Reflection;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

using Server.Data;

public sealed class Program {
    private static void Main(string[] args) {
        InitApplication(ConfigureApplication(args)).Run();
    }

    private static WebApplication InitApplication(WebApplication app) {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        return app;
    }

    private static WebApplication ConfigureApplication(String[] args) {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Environment.WebRootPath = "static";

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));

        builder.Services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Version = "v1",
                Title = "API",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        var app = builder.Build();

        app.UseCors(b => {
            if(app.Environment.IsDevelopment()) {
                b.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            }
        });

        var filesDir = Path.Combine(
               AppDomain.CurrentDomain.BaseDirectory,
               builder.Environment.WebRootPath,
               "uploads");
        _ = Directory.CreateDirectory(filesDir);
        var staticFilesOptions = new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(filesDir),
            RequestPath = "/uploads"
        };

        app.UseStaticFiles(staticFilesOptions);

        if(app.Environment.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
        }

        app.UseHttpsRedirection();

        app.MapControllers();
        return app;
    }
}
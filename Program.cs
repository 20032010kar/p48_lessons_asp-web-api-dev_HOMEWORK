using System.Globalization;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using p48_lessons_asp.Data;
using p48_lessons_asp.Models;
using p48_lessons_asp.Services.Random;
using p48_lessons_asp.Services.Slug;
using Slugify;
using Serilog;

namespace p48_lessons_asp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true).Build();

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            try
            {
                Log.Information("Running web-application");
                var builder = WebApplication.CreateBuilder(args);
                ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
                
                
                builder.Host.UseSerilog();
                builder.Services.AddSingleton<IRandomService, RandomService>();
                builder.Services.AddSingleton<ISlugHelper, SlugHelper>();
                builder.Services.AddSingleton<ISlugService, SlugService>();

                builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                builder.Services.AddValidatorsFromAssemblyContaining<Program>();
                
                
                builder.Services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    });

                
                builder.Services.AddEndpointsApiExplorer();
                
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
                    {
                        Title = "ASP.Net Core WebAPI Lessons",
                        Version = "v1.0",
                        Description = "Documentation for REST API on ASP.Net Core",
                        Contact = new Microsoft.OpenApi.OpenApiContact
                        {
                            Name = "dsgnrr",
                            Email = "dsgnrr@rnicrosoft-supprot.com"
                        }
                    });
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                    c.IncludeXmlComments(xmlPath);
                    c.EnableAnnotations();
                });

                var connectionString = builder.Configuration.GetConnectionString("MSSqlConnection");
                if (connectionString == null)
                {
                    throw new Exception("No connection string. Please provide in appsettings.json");
                }
                try
                {
                    builder.Services.AddDbContext<DataContext>(
                        options => options.UseSqlServer(connectionString)
                    );
                }
                catch (Exception)
                {
                    throw new Exception("Getting error while trying to connect to database");
                }
                var app = builder.Build();

                app.UseSerilogRequestLogging();
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();

                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application stopped with fatal error");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

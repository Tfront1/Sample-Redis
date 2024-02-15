using Microsoft.EntityFrameworkCore;
using Sample_Redis.Data;
using Sample_Redis.Services;
using System.Reflection;

namespace Sample_Redis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddEntityFrameworkNpgsql()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                string projectName = 
                ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                typeof(AssemblyTitleAttribute))).Title; //Or just define it in appsettings)
                string connectionString = builder.Configuration.GetConnectionString("Redis");

                options.Configuration = connectionString;
                options.InstanceName = projectName;
            });

            builder.Services.AddSingleton<ICacheService, CacheService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
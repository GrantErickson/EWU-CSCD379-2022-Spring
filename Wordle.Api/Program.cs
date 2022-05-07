using Wordle.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Wordle.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Wordle.Api.Hubs;

namespace Wordle.Api
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            var allOrigins = "AllOrigins";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: allOrigins,
                    policy => policy
                        //.AllowAnyOrigin()
                        .WithOrigins("http://localhost:3000", "https://mango-glacier-08064d21e.1.azurestaticapps.net")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
            });

            var jwtConfiguration = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>();
            builder.Services.AddSingleton(jwtConfiguration);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add identity
            builder.Services.AddIdentityCore<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfiguration.Issuer,
                        ValidAudience = jwtConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret!))
                    };
                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnAuthenticationFailed = context =>
                    //    {
                    //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    //        {
                    //            context.Response.Headers.Add("Token-Expired", "true");
                    //        }
                    //        return Task.CompletedTask;
                    //    }
                    //};
                });

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Wordle API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Description = "Specify authorization token",
                    Type = SecuritySchemeType.Http
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
                });
                c.AddSignalRSwaggerGen();
            });

            builder.Services.AddScoped<ILeaderBoardService, LeaderBoardService>();

            var app = builder.Build();

            // Run Migrations on the datbase automatically on startup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                // Seed Data
                StatisticsService.Seed(db);
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors(allOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<GameHub>("/gamehub");

            app.MapControllers();

            app.Run();
        }
    }
}
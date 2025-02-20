using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestAPIadminPortal.Data;

namespace TestAPIadminPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                    policy => policy.WithOrigins("http://localhost:3000") // Allow the React app
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });
            builder.Services.AddEndpointsApiExplorer();



            builder.Services.AddSwaggerGen(); // This adds Swagger generator


            //Authentication JwtBearer


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {           
                    ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                          ClockSkew = TimeSpan.Zero
                 };
                });




            // Register the database context with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            var app = builder.Build();
            app.UseCors("AllowLocalhost");

            app.UseAuthentication();
            /// swagger / index.html
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
                app.UseSwagger(); // Adds the Swagger endpoint
                app.UseSwaggerUI(); // Adds the Swagger UI interface
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

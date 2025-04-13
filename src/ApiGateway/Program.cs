using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Context;
using Serilog.Sinks.Elasticsearch;
using System.Text.Json;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 1. Configure Serilog as the logging provider
            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
             .MinimumLevel.Override("Ocelot", LogEventLevel.Information)
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .WriteTo.File(
                 path: "Logs/apigateway-.log",
                 rollingInterval: RollingInterval.Day,
                 retainedFileCountLimit: 7,
                 outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"
             )
             .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
             {
                 AutoRegisterTemplate = true,
                 IndexFormat = "apigateway-logs-{0:yyyy.MM.dd}"
             })
             .CreateLogger();

            try
            {
                Log.Information("Starting API Gateway");

                var builder = WebApplication.CreateBuilder(args);

                // 2. Tell ASP.NET Core to use Serilog
                builder.Host.UseSerilog();

                // Load Ocelot configuration
                builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

                // Add Ocelot services with configuration
                builder.Services.AddOcelot(builder.Configuration);

                // Register Swagger services for API documentation (if needed)
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Configure JWT Authentication
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Bearer";
                    options.DefaultChallengeScheme = "Bearer";
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

                // Add authorization
                builder.Services.AddAuthorization();

                // If you want to add CORS (even if no frontend exists right now)
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                var app = builder.Build();

                // 3. Correlation ID middleware
                app.Use(async (context, next) =>
                {
                    var correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers["X-Correlation-ID"] = correlationId;
                    LogContext.PushProperty("CorrelationId", correlationId);
                    await next();
                });

                // Global error handling middleware
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Unhandled exception");
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred.", details = ex.Message });
                    }
                });

                // Enable CORS if needed
                app.UseCors("AllowAll");

                // Enable authentication and authorization middleware
                app.UseAuthentication();
                app.UseAuthorization();

                // Short‑circuit /issue-token before Ocelot
                app.MapWhen(ctx =>
                    ctx.Request.Path.Equals("/issue-token", StringComparison.OrdinalIgnoreCase),
                    builderBranch =>
                    {
                        builderBranch.Run(async context =>
                        {
                            var config = context.RequestServices.GetRequiredService<IConfiguration>();
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var token = new JwtSecurityToken(
                                issuer: config["Jwt:Issuer"],
                                audience: config["Jwt:Audience"],
                                expires: DateTime.UtcNow.AddMinutes(30),
                                signingCredentials: creds);

                            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new { token = jwt }));
                        });
                    });
                // Protected test endpoint
                app.MapGet("/jwt-test", [Microsoft.AspNetCore.Authorization.Authorize] () =>
                {
                    return Results.Ok("JWT valid!");
                });

                // Enable Swagger in development environment
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Finally, use Ocelot middleware for routing
                await app.UseOcelot();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "API Gateway terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

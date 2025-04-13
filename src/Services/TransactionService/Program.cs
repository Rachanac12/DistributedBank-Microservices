
using FluentValidation.AspNetCore;
using FluentValidation;
using MediatR;
using TransactionService.Application.Validators;
using TransactionService.Application.Mappings;
using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Repositories;
using System.Reflection;
using Serilog.Events;
using Serilog;
using Serilog.Context;

namespace TransactionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. Configure Serilog as the logging provider
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Ocelot", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/transaction-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            try
            {
                Log.Information("Starting Transaction Service");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Host.UseSerilog();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddDbContext<TransactionDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionDb")));

                builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
                builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
                builder.Services.AddAutoMapper(typeof(MappingProfile));
                builder.Services.AddFluentValidationAutoValidation();
                builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionCommandValidator>();

                var app = builder.Build();

                app.Use(async (context, next) =>
                {
                    var correlationId = Guid.NewGuid().ToString();
                    context.Request.Headers["X-Correlation-ID"] = correlationId;
                    LogContext.PushProperty("CorrelationId", correlationId);
                    await next();
                });

                // 5. Logging middleware
                app.Use(async (ctx, next) =>
                {
                    Log.Information("Handling {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
                    await next();
                    Log.Information("Finished {Method} {Path} responded {StatusCode}", ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode);
                });

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

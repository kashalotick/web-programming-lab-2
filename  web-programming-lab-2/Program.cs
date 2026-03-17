using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Middlewares;

namespace web_programming_lab_2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load("../");


        // swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // database
        var connectionString = GenerateConnectionString();
        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        // automapper
        builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); }, typeof(MappingProfile));
        // validators
        builder.Services.AddValidatorsFromAssemblyContaining<ReservationDtoCreate>();
        builder.Services.AddFluentValidationAutoValidation();
        // services
        builder.Services.AddScoped<RoomService>();
        builder.Services.AddScoped<ReservationService>();

        // controllers
        builder.Services.AddControllers();
        

        var app = builder.Build();

        // middlewares
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static string GenerateConnectionString()
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = $"Host={host};Database={database};Username={user};Password={password}";
        return connectionString;
    }
}
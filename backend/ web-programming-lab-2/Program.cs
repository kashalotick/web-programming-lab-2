using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Middlewares;

namespace web_programming_lab_2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load("../");
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevCorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000") // Адреса твого Nuxt фронтенда
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // Потрібно, якщо передаєш куки або Auth заголовки
            });
        });
        
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
        builder.Services.AddScoped<IValidator<ReservationCreateRequest>, ReservationCreateRequestValidator>();

        builder.Services.AddValidatorsFromAssemblyContaining<ReservationDtoCreate>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReservationCreateRequest>();
        builder.Services.AddValidatorsFromAssemblyContaining<GuestDtoCreate>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReservationDtoUpdate>();
        builder.Services.AddValidatorsFromAssemblyContaining<TimePeriodFilter>();
        builder.Services.AddFluentValidationAutoValidation();

        // services
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<GuestService>();
        builder.Services.AddScoped<RoomService>();
        builder.Services.AddScoped<ReservationService>();

        // controllers
        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseRouting();
        app.UseCors("DevCorsPolicy");
        
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
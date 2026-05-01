using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.UseCases;
using BMS.Domain.Events;
using MediatR;
using BMS.Infrastructure.Alarm;
using BMS.Infrastructure;
using BMS.Infrastructure.Events;
using BMS.Infrastructure.Historian;
using BMS.Infrastructure.Realtime;
using BMS.Infrastructure.Realtime.Hubs;
using System.Threading.Channels;
using WebApi.Domain.Twin.Services;
using WebApi.Infrastructure.Twin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Realtime.Extensions;
using WebApi.Realtime.Hubs;

using Microsoft.Extensions.DependencyInjection;
using BMS.Infrastructure.State;
using BMS.Application;
using FluentValidation;
using Microsoft.OpenApi.Models;
using BMS.Application.Common.Settings;
using BMS.Application.Common.Interfaces;
using BMS.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using BMS.Infrastructure.Persistence;
using BMS.Application.Common.Behaviors;
using BMS.Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(typeof(ApplicationAssemblyReference).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRealtimeInfrastructure();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BMS.API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
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
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // آدرس فرانت اند انگولار
              .AllowAnyMethod()                   // اجازه دادن به هر متدی (GET, POST, PUT, DELETE)
              .AllowAnyHeader()                   // اجازه دادن به هر هدر
              .AllowCredentials();                // اجازه دادن به اعتبارسنجی (اگر نیاز است)
    });
});


//builder.Services.AddMediatR();
builder.Services.AddSingleton<ITwinRepository, InMemoryTwinRepository>();
//builder.Services.AddScoped<ITwinService, TwinService>();
//builder.Services.AddScoped<IDeviceStateStore, DeviceStateStore >();
builder.Services.AddSingleton<IDeviceStateStore, InMemoryDeviceStateStore>();
builder.Services.AddScoped<IHistorianWriter, ChannelHistorianWriter>();
//builder.Services.AddScoped<Channel, ChannelHistorianWriter>();
//builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// --------------------
// JWT Settings Binding
// --------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

// --------------------
// JWT Provider
// --------------------
builder.Services.AddSingleton<IJwtProvider>(sp =>
{
    var settings = sp
        .GetRequiredService<IOptions<JwtSettings>>()
        .Value;

    return new JwtProvider(settings);
});

// --------------------
// Authentication / Authorization
// --------------------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var settings = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtSettings>()!;

        options.TokenValidationParameters =new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // حرفه‌ای‌تر

                ValidIssuer = settings.Issuer,
                ValidAudience = settings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.Key)
                    )
            };
    });
builder.Services.AddAuthorization();
// Permission system
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

//builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();


builder.Services.AddScoped<UpdateDataPointUseCase>();

builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<ExecuteCommandUseCase>();
builder.Services.AddScoped<
    IEventHandler<DataPointUpdatedDomainEvent>,
    DataPointUpdatedRealtimeHandler>();

builder.Services.AddScoped<
    IEventHandler<GetDeviceStateDomainEvent>,
    GetDeviceStateRealtimeHandler>();

var channel = Channel.CreateUnbounded<DataPointDeltaModel>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

builder.Services.AddSingleton(channel);

//builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

// --------------------
// Infrastructure
// --------------------
//var connectionString = builder.Configuration
//    .GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string not found.");


// --------------------
// Pipeline Behaviors
// --------------------
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(AuditLoggingBehavior<,>)
);
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddInfrastructure2(connectionString);

// --------------------
// DbContext
// --------------------
//builder.Services.AddDbContext<BMSDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        x => x.MigrationsAssembly("Bms.Infrastructure")
//    )
//);
builder.Services.AddScoped<IEventHandler<DataPointUpdatedDomainEvent>,
    DataPointUpdatedHistorianHandler>();

//builder.Services.AddScoped<IEventHandler<DataPointUpdatedDomainEvent>,
//    DataPointUpdatedAlarmHandler>();
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAngularDev");
app.MapHub<BMSHub>("/hubs/deviceState");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
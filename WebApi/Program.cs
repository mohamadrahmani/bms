using BMS.Application.Interfaces;
using BMS.Application.Models;
using BMS.Application.UseCases;
using BMS.Domain.Events;
using BMS.Infrastructure.Alarm;
using BMS.Infrastructure.Events;
using BMS.Infrastructure.Historian;
using BMS.Infrastructure.Realtime;
using BMS.Infrastructure.Realtime.Hubs;
using System.Threading.Channels;
using WebApi.Domain.Twin.Services;
using WebApi.Infrastructure.Twin;
using WebApi.Realtime.Extensions;
using WebApi.Realtime.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRealtimeInfrastructure();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddSingleton<ITwinRepository, InMemoryTwinRepository>();
//builder.Services.AddScoped<ITwinService, TwinService>();
builder.Services.AddScoped<IDeviceStateStore, DeviceStateStore >();
builder.Services.AddScoped<IHistorianWriter, ChannelHistorianWriter>();
//builder.Services.AddScoped<Channel, ChannelHistorianWriter>();
//builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

builder.Services.AddScoped<UpdateDataPointUseCase>();

builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();

builder.Services.AddScoped<
    IEventHandler<DataPointUpdatedDomainEvent>,
    DataPointUpdatedRealtimeHandler>();

var channel = Channel.CreateUnbounded<DataPointDeltaModel>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

builder.Services.AddSingleton(channel);

//builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

builder.Services.AddScoped<IEventHandler<DataPointUpdatedDomainEvent>,
    DataPointUpdatedHistorianHandler>();

//builder.Services.AddScoped<IEventHandler<DataPointUpdatedDomainEvent>,
//    DataPointUpdatedAlarmHandler>();
var app = builder.Build();
app.UseCors("AllowAngularDev");
app.MapHub<BMSHub>("/hubs/twin");
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

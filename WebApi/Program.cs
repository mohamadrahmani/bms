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
builder.Services.AddScoped<ITwinService, TwinService>();

var app = builder.Build();
app.UseCors("AllowAngularDev");
app.MapHub<TwinHub>("/hubs/twin");
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

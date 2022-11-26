using WebapiGateway.Models;
using WebapiGateway.Mqtt;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Cache<Point>>();
builder.Services.AddSingleton<Cache<Signal>>();

// TODO: Connection details should be loaded from a config provider
builder.Services
    .AddSingleton(static services => new MQTTService(services.GetRequiredService<ILogger<MQTTService>>(), "webapi", "localhost", 1883, "richard", "password", "signalling/", true, true))
    .AddHostedService(static services => (MQTTService) services.GetRequiredService<MQTTService>());

builder.Services.AddHostedService<PointStatusMessageHandler>();
builder.Services.AddHostedService<SignalStatusMessageHandler>();

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

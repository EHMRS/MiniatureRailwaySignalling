using System.Text.Json;
using MRS.ApiGateway.Models;
using MRS.ApiGateway.Mqtt;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase))
);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Cache<Point>>();
builder.Services.AddSingleton<Cache<Signal>>();
builder.Services.AddSingleton<Cache<Tracksection>>();

// TODO: Connection details should be loaded from a config provider
builder.Services
    .AddSingleton(static services => new MQTTService(services.GetRequiredService<ILogger<MQTTService>>(), "webapi", "192.168.255.6", 1883, "richard", "password", "signalling/", true, true))
    .AddHostedService(static services => (MQTTService) services.GetRequiredService<MQTTService>());

builder.Services.AddHostedService<PointStateMessageHandler>();
builder.Services.AddHostedService<SignalStateMessageHandler>();
//builder.Services.AddHostedService<TracksectionStateMessageHandler>();

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

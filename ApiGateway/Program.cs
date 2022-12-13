using System.Text.Json;
using MRS.ApiGateway.Models;
using MRS.ApiGateway.Mqtt;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(
        configure: static x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
    );

builder.Services.AddRouting(static options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Cache<Point>>();
builder.Services.AddSingleton<Cache<Signal>>();
builder.Services.AddSingleton<Cache<Tracksection>>();

builder.Services
    .AddSingleton(
        services => new MQTTService(
            services.GetRequiredService<ILogger<MQTTService>>(),
            "ApiGateway",
            builder.Configuration["MQTT_HOST"] ?? "localhost",
            int.Parse(builder.Configuration["MQTT_PORT"] ?? "1883"),
            builder.Configuration["MQTT_USER"] ?? "webapi",
            builder.Configuration["MQTT_PASSWORD"] ?? "password",
            builder.Configuration["MQTT_PREFIX"] ?? "signalling/",
            true,
            true
        )
    )
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

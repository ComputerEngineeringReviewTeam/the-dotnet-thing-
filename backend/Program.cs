using Services;
using Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "permissive",
                      builder =>
                      {
                          builder.WithOrigins("*");
                      });
});

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHostedService<MqttService>();
builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

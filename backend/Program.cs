var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers(); 
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
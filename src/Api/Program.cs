using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Регистрация Application Layer
builder.Services.AddApplication();

// Регистрация Infrastructure Layer (включая DbContext)
builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
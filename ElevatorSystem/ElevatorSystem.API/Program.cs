using ElevatorSystem.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register elevator services
builder.Services.AddSingleton<Elevator>(provider => 
    new Elevator(10, provider.GetRequiredService<ILogger<Elevator>>()));
builder.Services.AddSingleton<IElevatorController, ElevatorController>();

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

// Public class for integration tests
public partial class Program { }

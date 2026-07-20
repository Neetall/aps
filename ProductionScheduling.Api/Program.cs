using ProductionScheduling.Api.Services;
using ProductionScheduling.Application;

var builder = WebApplication.CreateBuilder(args);


// MVC Controller
builder.Services.AddControllers();


// APS Service
builder.Services.AddProductionScheduling();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();

var app = builder.Build();

app.MapControllers();


app.Run();
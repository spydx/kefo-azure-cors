var builder = WebApplication.CreateBuilder(args);
const string CORS_ENVIRONMENT = "LOCALCORS";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

var env = Environment.GetEnvironmentVariable(CORS_ENVIRONMENT);
if (env is not null)
{
    // set CORS Settings
    app.UseCors();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

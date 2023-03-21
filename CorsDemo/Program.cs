var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure Cors Policy
builder.Services.AddCors(p => p
    .AddDefaultPolicy(cors => cors
        .WithOrigins("https://local.kefo.no")
        .AllowAnyHeader()
        .AllowAnyMethod())
     );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


// set CORS Settings

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

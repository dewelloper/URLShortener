var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//string allowedOrigins = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["AllowedOrigins"];


builder.Services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//give access to any origin for testing purposes
app.UseCors("AllowAnyOrigin");

app.UseAuthorization();

app.MapControllers();

app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});

app.Run();

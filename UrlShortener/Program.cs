/// writer: Hamit Yıldırım </summary>
/// date: 18.09.2021 </summary>
/// time: 23:00 </summary>
/// this project prepared for the interview of the company named "XXX" </summary>
/// There are many urlshorterner implementations in the world. </summary>
/// I just aimed to prepare main logic of the urlshorterner. </summary>
/// It needs many improvements. </summary>
/// any further questions or suggestions, please contact me. 
/// email: dewelloper@gmail.com or yildirim.hamit@hotmail.com</summary>


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

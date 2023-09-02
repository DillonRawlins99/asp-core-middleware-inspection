var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use middleware inspection to inspect the incoming JSON data before it reaches the action method.
app.Use(async (context, next) =>
{
   // https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body
   context.Request.EnableBuffering();
   // Little lambda one-liner to convert a stream to a string in-memory
   Func<Stream,Task<string>> streamToString = async (Stream stream) => await new StreamReader(stream).ReadToEndAsync();
   var requestString = await streamToString(context.Request.Body);
   // Seek back to beginning so other middleware can read the request body
   context.Request.Body.Seek(0,SeekOrigin.Begin);
   Console.WriteLine($"Request Body:\n{requestString}");
   await next(context);
});

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

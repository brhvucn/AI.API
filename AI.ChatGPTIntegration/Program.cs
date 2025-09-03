using AI.ChatGPTIntegration.Models.Config;
using AI.ChatGPTIntegration.Services;
using AI.ChatGPTIntegration.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
//service registrations
builder.Services.AddScoped<IHttpService, HttpService>();
//build configurations
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));

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

app.MapGet("/", ()=> "Go to  /swagger");

app.Run();

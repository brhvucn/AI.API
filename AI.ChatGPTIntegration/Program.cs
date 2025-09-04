using AI.ChatGPTIntegration.Agents;
using AI.ChatGPTIntegration.DAL;
using AI.ChatGPTIntegration.Models.Config;
using AI.ChatGPTIntegration.Models.Requests;
using AI.ChatGPTIntegration.Services;
using AI.ChatGPTIntegration.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
//service registrations for the DI container
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<FAQRepository>();
builder.Services.AddScoped<SimpleAgent>();
//build configurations
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI")); //map to the section in the appsettings.json file

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

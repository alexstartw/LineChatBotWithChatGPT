using ChatGPT.Net.DTO;
using LineChatBotWithChatGPT.Interfaces;
using LineChatBotWithChatGPT.Models;
using LineChatBotWithChatGPT.Services;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IChatGptService,ChatGptService>();
builder.Services.Configure<LineBotToken>(builder.Configuration.GetSection("LineBot"));
builder.Services.Configure<ChatGptToken>(builder.Configuration.GetSection("ChatGPTSession"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LineBot}/{action=Index}");


app.Run();
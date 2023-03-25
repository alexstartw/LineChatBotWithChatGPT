using ChatGPT.Net.DTO;
using LineChatBotWithChatGPT.Configs;
using LineChatBotWithChatGPT.Interfaces;
using LineChatBotWithChatGPT.Models;
using LineChatBotWithChatGPT.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IChatGptService,ChatGptService>();
builder.Services.Configure<LineBotConfig>(builder.Configuration.GetSection("LineBot"));
builder.Services.Configure<ChatGptSession>(builder.Configuration.GetSection("ChatGPTSession"));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LineBot}/{action=Index}");

app.Run();
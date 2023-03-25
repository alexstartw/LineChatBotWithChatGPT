using LineChatBotWithChatGPT.Configs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<LineBotConfig>(builder.Configuration.GetSection("LineBot"));


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
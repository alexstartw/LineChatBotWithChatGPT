using LineChatBotWithChatGPT.Configs;

namespace LineChatBotWithChatGPT;

public class StartUp
{
    private IConfiguration Configuration { get; }
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSingleton<LineBotConfig, LineBotConfig>(_ => new LineBotConfig
        {
            ChannelSecret = Configuration["LineBotConfig:ChannelSecret"],
            AccessToken = Configuration["LineBotConfig:AccessToken"]
        });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.UseRouting();
        
        //
        // app.MapControllerRoute(
        //     name: "default",
        //     pattern: "{controller=LineBot}/{action=Index}");
        //
        // app.Run();
    }
}
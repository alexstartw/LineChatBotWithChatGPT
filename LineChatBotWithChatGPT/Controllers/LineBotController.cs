using System.Text;
using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Session;
using Line.Messaging;
using LineChatBotWithChatGPT.Interfaces;
using LineChatBotWithChatGPT.Models;
using LineChatBotWithChatGPT.Services;
using LineChatBotWithChatGPT.Sources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace LineChatBotWithChatGPT.Controllers;
// [Route("api/[controller]/[action]")]
[ApiController]
[Route("api/linebot")]
public class LineBotController: Controller
{
    private readonly LineBotToken _lineBotToken;
    private readonly ChatGptToken _chatGptToken;
    private readonly IChatGptService _chatGptService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext _httpContext;
    private readonly ILogger<LineBotController> _logger;

    public LineBotController(IServiceProvider serviceProvider, IChatGptService chatGptService,
        IOptionsMonitor<LineBotToken> lineBotConfig, IOptionsMonitor<ChatGptToken> chatGptSession, ILogger<LineBotController> logger)
    {
        _chatGptService = chatGptService;
        _logger = logger;
        _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        _httpContext = _httpContextAccessor.HttpContext!;
        _chatGptToken = chatGptSession.CurrentValue;
        _lineBotToken = lineBotConfig.CurrentValue;
    }

    [HttpPost("run")]
    public async Task<IActionResult> Post()
    {
        try
        {
            var webhookEventsAsync = await _httpContext.Request.GetWebhookEventsAsync(_lineBotToken.ChannelSecret);

            var lineMessagingClient = new LineMessagingClient(_lineBotToken.AccessToken);
            var lineBotApp = new LineBotAppService(lineMessagingClient,_logger,_chatGptService);
            await lineBotApp.RunAsync(webhookEventsAsync);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }

        return Ok();
    }


    [HttpPost]
    public async Task<string> QuestionToChatGpt(string question)
    {
        var chatGptClient = await _chatGptService.CreateChatGptClient(_chatGptToken.SessionToken);
        var response = await chatGptClient.Ask(question);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return response;
    }
}
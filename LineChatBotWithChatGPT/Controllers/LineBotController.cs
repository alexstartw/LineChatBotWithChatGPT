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

    public LineBotController(IServiceProvider serviceProvider, IChatGptService chatGptService,
        IOptionsMonitor<LineBotToken> lineBotConfig, IOptionsMonitor<ChatGptToken> chatGptSession)
    {
        _chatGptService = chatGptService;
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
            var lineBotApp = new LineBotAppService(lineMessagingClient);
            await lineBotApp.RunAsync(webhookEventsAsync);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return Ok();
    }


    [HttpPost]
    public async Task<string> QuestionToChatGpt(string question)
    {
        var response = await (await _chatGptService.CreateChatGptClient(_chatGptToken.SessionToken)).Ask(question);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return response;
    }
}
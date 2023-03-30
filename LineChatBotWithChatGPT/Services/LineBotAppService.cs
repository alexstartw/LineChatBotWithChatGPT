using Line.Messaging;
using Line.Messaging.Webhooks;
using LineChatBotWithChatGPT.Controllers;
using LineChatBotWithChatGPT.Interfaces;


namespace LineChatBotWithChatGPT.Services;

public class LineBotAppService : WebhookApplication
{
    private readonly LineMessagingClient _messagingClient;
    private readonly IChatGptService _chatGptService;
    private readonly ILogger<LineBotController> _logger;

    public LineBotAppService(LineMessagingClient lineMessagingClient, ILogger<LineBotController> logger, IChatGptService chatGptService)
    {
        _messagingClient = lineMessagingClient;
        _logger = logger;
        _chatGptService = chatGptService;
    }

    protected override async Task OnMessageAsync(MessageEvent ev)
    {
        var result = null as List<ISendMessage>;

        switch (ev.Message)
        {
            //文字訊息
            case TextEventMessage textMessage:
            {
                
                var channelId = ev.Source.Id;

                var userId = ev.Source.UserId;

                var chatGptResponse = _chatGptService.ChatGptResponse(textMessage);
                result = new List<ISendMessage>
                {
                    new TextMessage(chatGptResponse.Content)
                };
                
                
                

                // if (textMessage.Text == "test")
                // {
                //     result = new List<ISendMessage>
                //     {
                //         new TextMessage("testing")
                //     };
                // }
                // else
                // {
                //     result = new List<ISendMessage>
                //     {
                //         new TextMessage("hellow")
                //     };
                // }

            }
                break;
        }

        if (result != null)
            await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
    }

}

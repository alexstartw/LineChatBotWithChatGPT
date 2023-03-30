using Line.Messaging;
using Line.Messaging.Webhooks;
using LineChatBotWithChatGPT.Controllers;
using LineChatBotWithChatGPT.Interfaces;
using RestSharp;


namespace LineChatBotWithChatGPT.Services;

public class LineBotAppService : WebhookApplication
{
    private readonly LineMessagingClient _messagingClient;
    private readonly IChatGptService _chatGptService;
    private static ILogger _logger;

    public LineBotAppService(LineMessagingClient lineMessagingClient, IChatGptService chatGptService, ILogger<LineBotController> logger)
    {
        _messagingClient = lineMessagingClient;
        _chatGptService = chatGptService;
        _logger = logger;
    }

    protected override async Task OnMessageAsync(MessageEvent ev)
    {
        var result = null as List<ISendMessage>;

        switch (ev.Message)
        {
            //文字訊息
            case TextEventMessage textMessage:
            {
                //頻道Id
                var channelId = ev.Source.Id;
                //使用者Id
                var userId = ev.Source.UserId;

                if (textMessage.Text == "test")
                {
                    result = new List<ISendMessage>
                    {
                        new TextMessage("testing")
                    };
                }
                else
                {
                    result = new List<ISendMessage>
                    {
                        new TextMessage("hellow")
                    };
                }

            }
                break;
        }

        if (result != null)
            await _messagingClient.ReplyMessageAsync(ev.ReplyToken, result);
    }

    private static RestResponse RestResponse(TextEventMessage textMessage)
    {
        _logger.LogInformation($"############################################################################");
        string apiUrl = "https://api.openai.com/v1/engines/text-davinci-003/completions";
        string apiKey = "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";
        var client = new RestClient(apiUrl);
        var request = new RestRequest(apiUrl, Method.Post);
        request.AddHeader("Authorization", "Bearer " + apiKey);
        request.AddJsonBody(new
        {
            prompt = textMessage.Text,
            max_tokens = 100,
            n = 1,
            temperature = 0.5,
        });
        var response = client.Execute(request);

        _logger.LogInformation($"Answer: {response.Content}");
        _logger.LogInformation($"*****************************************************************************");
        return response;
    }
}

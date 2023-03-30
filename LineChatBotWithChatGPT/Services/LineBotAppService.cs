using System.Text;
using Line.Messaging;
using Line.Messaging.Webhooks;
using LineChatBotWithChatGPT.Models;
using Microsoft.Extensions.Options;

namespace LineChatBotWithChatGPT.Services;

public class LineBotAppService : WebhookApplication
{
    private readonly LineMessagingClient _messagingClient;
    private readonly ChatGptToken _chatGptToken;
    private readonly ChatGptService _chatGptService = new();

    public LineBotAppService(LineMessagingClient lineMessagingClient, ChatGptToken chatGptToken)
    {
        _messagingClient = lineMessagingClient;
        _chatGptToken = chatGptToken;
    }

    protected override async Task OnMessageAsync(MessageEvent ev)
    {
        var result = null as List<ISendMessage>;
        var chatGptClient = await _chatGptService.CreateChatGptClient(_chatGptToken.SessionToken);

        switch (ev.Message)
        {
            //文字訊息
            case TextEventMessage textMessage:
            {
                //頻道Id
                var channelId = ev.Source.Id;
                //使用者Id
                var userId = ev.Source.UserId;

                var response = await chatGptClient.Ask(textMessage.Text);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                result = new List<ISendMessage>
                {
                    new TextMessage(response)
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
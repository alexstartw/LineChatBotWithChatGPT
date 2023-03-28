using Line.Messaging;
using Line.Messaging.Webhooks;

namespace LineChatBotWithChatGPT.Services;

public class LineBotAppService : WebhookApplication
{
    private readonly LineMessagingClient _messagingClient;
    public LineBotAppService(LineMessagingClient lineMessagingClient)
    {
        _messagingClient = lineMessagingClient;
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
}
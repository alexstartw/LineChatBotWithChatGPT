using ChatGPT.Net.Session;
using Line.Messaging.Webhooks;
using RestSharp;

namespace LineChatBotWithChatGPT.Interfaces;

public interface IChatGptService
{
    public Task<ChatGptClient> CreateChatGptClient(string sessionToken);
    public RestResponse ChatGptResponse(TextEventMessage textMessage);
}
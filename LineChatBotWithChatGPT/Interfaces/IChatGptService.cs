using ChatGPT.Net.Session;

namespace LineChatBotWithChatGPT.Interfaces;

public interface IChatGptService
{
    public Task<ChatGptClient> CreateChatGptClient(string sessionToken);
}
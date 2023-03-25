using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Session;
using LineChatBotWithChatGPT.Interfaces;
using LineChatBotWithChatGPT.Models;

namespace LineChatBotWithChatGPT.Services;

public class ChatGptService : IChatGptService
{


    public async Task<ChatGptClient> CreateChatGptClient(string sessionToken)
    {
        var chatGpt = new ChatGpt();
        await chatGpt.WaitForReady();

        return await chatGpt.CreateClient(new ChatGptClientConfig
        {
            SessionToken = sessionToken
        });
    }
}
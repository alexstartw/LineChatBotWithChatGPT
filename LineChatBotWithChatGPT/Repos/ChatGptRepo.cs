using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Session;

namespace LineChatBotWithChatGPT.Repos;

public class ChatGptRepo
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
using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Session;
using Line.Messaging.Webhooks;
using LineChatBotWithChatGPT.Interfaces;
using RestSharp;

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

    public RestResponse ChatGptResponse(TextEventMessage textMessage)
    {
        const string apiUrl = "https://api.openai.com/v1/engines/text-davinci-003/completions";
        const string apiKey = "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";
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

        return client.Execute(request);
    }

}
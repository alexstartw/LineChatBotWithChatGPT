using System.Security.Cryptography;
using System.Text;
using Line.Messaging;
using Line.Messaging.Webhooks;
using Newtonsoft.Json;

namespace LineChatBotWithChatGPT.Sources;

/*
MIT License

Copyright (c) 2017 pierre3

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

public static class WebhookRequestMessageHelper
{
    public static async Task<IEnumerable<WebhookEvent>> GetWebhookEventsAsync(this HttpRequest request, string channelSecret, string botUserId = null)
    {
        if (request == null) { throw new ArgumentNullException(nameof(request)); }
        if (channelSecret == null) { throw new ArgumentNullException(nameof(channelSecret)); }

        var content = "";
        using (var reader = new StreamReader(request.Body))
        {
            content = await reader.ReadToEndAsync();
        }

        var xLineSignature = request.Headers["X-Line-Signature"].ToString();
        if (string.IsNullOrEmpty(xLineSignature) || !VerifySignature(channelSecret, xLineSignature, content))
        {
            throw new InvalidSignatureException("Signature validation faild.");
        }

        dynamic json = JsonConvert.DeserializeObject(content);

        if (!string.IsNullOrEmpty(botUserId))
        {
            if (botUserId != (string)json.destination)
            {
                throw new UserIdMismatchException("Bot user ID does not match.");
            }
        }
        return WebhookEventParser.ParseEvents(json.events);
    }

    internal static bool VerifySignature(string channelSecret, string xLineSignature, string requestBody)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(channelSecret);
            var body = Encoding.UTF8.GetBytes(requestBody);

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(body, 0, body.Length);
                var xLineBytes = Convert.FromBase64String(xLineSignature);
                return SlowEquals(xLineBytes, hash);
            }
        }
        catch
        {
            return false;
        }
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }
}
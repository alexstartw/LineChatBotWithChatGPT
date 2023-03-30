using System.Text;
using Line.Messaging;
using Line.Messaging.Webhooks;
using LineChatBotWithChatGPT.Interfaces;
using LineChatBotWithChatGPT.Models;
using Microsoft.Extensions.Options;

namespace LineChatBotWithChatGPT.Services;

public class LineBotAppService : WebhookApplication
{
    private readonly LineMessagingClient _messagingClient;
    private readonly IChatGptService _chatGptService;

    public LineBotAppService(LineMessagingClient lineMessagingClient, IChatGptService chatGptService)
    {
        _messagingClient = lineMessagingClient;
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
                //頻道Id
                var channelId = ev.Source.Id;
                //使用者Id
                var userId = ev.Source.UserId;

                var chatGptClient = await _chatGptService.CreateChatGptClient(
                    "eyJhbGciOiJkaXIiLCJlbmMiOiJBMjU2R0NNIn0..VtcuH2m3vTQugk9v.PVQXsps1AK7yPZsRWhnHp02qRdk-ed7ovUsdp9tE-jWY5MOXr-agJ6o1LB03Fx6YxuEI6E_ZkRt2WIEUPmyM0fU901lvq5VvC2pFtulGQIjR8gaypPg-E64FC5AXD-E0GPgUTG2MKCCwu02gqyVoVfo6pa7tQ8VGdJk86_lXGihQ70LkFimLAuwH1Mc_zLCxsxKP_u0j0raK4V7ywjYmerqNaPp2xtWdtmzHDNMWH9_wpr3owLfQS2tNO6oHBoUOD1OOWaGhIFgY1Vlom2kcH-YQOuK5Bf6yoqmapsUuTsAb7rl8tUZtc2riy2MKMlQv0AOn0g9dLMwETIsANzNQS9uOVRUJN3_sKAO6kwJc0QpqPOKh1rCKzV4kdBzDJPCJsjZnvZhMu5VM2qk4PoGG6zIwDkd8ccin7T9tmcbRn8ugR7wvqG5b0obOJyhkOmRchEqg3inl_jGne0pDFy0iaDxWOjg7dCkv7Ze3W1FN3nr8HayPP47ejHMK4PPdEW7HtnHaRHBkYpJCoYcgZa-UOuYMFKQXwkdeETZiXzVd0E4qZfvtJ23p4DKRe6EbkQN2vZ7TENMMAC1yFdkL5FITGS1wn3L_p5qxds3qKwWHeZSDGJ4xWkMXvZuHf7LloMdlGiH2Bfg3bBDcWI3Pzz9oa8h27lMCgwd-klHf60ZecGZ4KYIBetYS0Uj7UWA_5iHEny6XAy66HJ3xICVvnanU8xzwDSVecRhFeKKgcQKHg0oH5qEKE333pbspj__WEyKQgxWGImCpEfsWyNEKWJ1dgNPmE-Odfwyck81ot0UHgJ1UBjPE8eYNuv_PDLTEggWRB4qq_HIcBGWU34ZB8_tiaCvoMxV8lvsHJ_Avm4xvu2ZAyuhkIWvOvBbaUbEs7sV0Xtt0mWC6W5DimrWLp8GqhWhDVKtShVV1PLYl1QDYUUHOkqXcIB2v9nP5lLdyjflbolZwvqRiVlL4ESQj3jM0XvbhB0W3sjxBmsmef5a-tFZU9NjFD4X9yArnCLNtkPIIZyesSvRJ1B16EFORXD8hphekkprQIa3_fs31M5u_xke7H5_Pl3WIHc4XTd2QPPnJl741K9-Y_dGurwCmlHtGWx5OPMXOdkyVyUCkCGeXUSPfKCQ1hMRxke5S5Zu7arGqNvX2XjDbTy8efxggCOjMVQLIYAe10W6uqEL8O1Od2L2ua5IPNh9VNCR2COR4l-odaK9fhQCxT0V_Y75_VPPoxvZH93oqHOHKvwvK-3adTrCZxhBVmhmGCFxgfDrI5BkNH37DxSeY_66FZ1v1BlnoU9jLEVrt65f7YNCGaQhihNUxwjv8LJUex25i-zdu2rNv24C_l2z7QT3ooqBrOrXIHJrOvVt3DWMfzsnG6dmpONtdREAoLhVGO1QmBVu56qvkd2IMx4LRNPT3tNuGZIBNlxWolrey6RLUH2arCxqhfLHT3w4_R-_Iqr-vAsjtzQ4EJpXcwZv9_t-xlhQ240q_cSzmIpd5Osy0ndclhDNLOPvMrGeV5RE_ZNOIW4QYAnR1tfkksNu6FDk7wTCLU2GPvjkl-jTJS0yxp76ZWWdJKqff3xK8TxUns2rCiQ3KovG2KjRPh4Eq27QmoxvjAVqfRe5obolD0RhG7PKFr7waGxoKzvxOPg5-SrTxG9Vb-qzbi2bpIUzvHESH_qUOgUsyf_5g5k2xiXlg1sKXG-ObIPXWPgh4jd4d93PfxE7j8gozRYnOlCIl-fvlv0hiO_AU81eUZnYuBC2jZP9Eh7plngczTKsNq3tdsZtRcX-bzzFjKld_LeqGt_nrRXUuQu_q2rMs7USaqx4KcGhoprjqRNrc_w0aRLhwSvm9se7FBXEqE4nvOm7qjWu4gYjc8x7vp3TsMH9R6XA5QE1QpHKv2SWQtd5ieGTjeHLcBBkhRgfTtbnwk5rCXQlc8bF-qfQU44fAtLRt4dUHxEMMlnPGOs25dXGxlb3TMz_VsoKzkTUvLwGVYESir_e7weGoXpAJniUnvOOQS0upwOXJmolI4K1cfiY_OmZCAPHK0w-kwrQbwtWNxEeHKEOfdfJKf0gymxNxu6qvDdGFbfmGUijKZ1fKBfEYpm4r_mEMao3exyyuBRd6QdQAo1wSpn-6VDdqKttDVVXOsw3ww2opKyFsI__W2yicFEnOodteomAzbJPnFIRansf8wufK4xyBMEA-LaK9Jwt1P3MMoPeYl_30NAJDXV_11q6BP9EXikSdPfQfGts-iDGoG1kcyi15MuVFYH25umnP0GdXqjY72Wqbf2nlQL2AIRXcb6prxPD6Obg8Nw8wGyYG2jXwdEH9ZjLdHEtpFkgN1wPu_dzrKXz20YURrHVldWTS2Mwpy4wcIihjiDjMSbTLIMyWfYfFWTpdXpLot0LmoxfCDe1oz_hmZxXiWCQ_MdboBcYWWRKxWlKI0sCegwIj3vKnGCsyRZCnd4PSSVxG7wr_y1rK2dZgy0u73uZLeBbssGLiBAvtDaOj96o.spWqHaTHDij5_6fAT3V4xw");
                var ans = await chatGptClient.Ask(textMessage.Text);
                result = new List<ISendMessage>
                {
                    new TextMessage(ans)
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
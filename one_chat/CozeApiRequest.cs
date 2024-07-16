using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace 链接API
{
    public class CozeApiRequest
    {
        private readonly string _personalAccessToken;
        private readonly string _botId;
        private readonly string _baseUrl = "https://api.coze.cn/open_api/v2";

        public CozeApiRequest(string personalAccessToken, string botId)
        {
             ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _personalAccessToken = personalAccessToken;
            _botId = botId;
        }

        public async Task<string> SendChatRequest(string conversationId, string user, string query)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _personalAccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                client.DefaultRequestHeaders.Host = "api.coze.cn";
                client.DefaultRequestHeaders.ConnectionClose = false; // equivalent to "keep-alive"

                var requestBody = new
                {
                    conversation_id = conversationId,
                    bot_id = _botId,
                    user = user,
                    query = query,
                    stream = false
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_baseUrl + "/chat", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }


        public class CozeApiResponse
        {
            [JsonProperty("messages")]
            public Message[] Messages { get; set; }

            [JsonProperty("conversation_id")]
            public string ConversationId { get; set; }

            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("msg")]
            public string Message { get; set; }
        }

        public class Message
        {
            [JsonProperty("role")]
            public string Role { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("content_type")]
            public string ContentType { get; set; }
        }
    }
}

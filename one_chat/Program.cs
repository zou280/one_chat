using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using System.Speech.Synthesis;

namespace 链接API
{
    class Program
    {



        static void Main()
        {
            // 这里添加程序的主要逻辑
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // 创建一个新的语音合成对象
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {

                // 设置语音的一些属性，例如语音和语速
                synthesizer.SelectVoice("Microsoft Huihui Desktop");
                synthesizer.Rate = 3; // 语速，范围从-10到+10，默认为0



                Console.Write(@"请回复序号选择LLM模型:(1--- 云雀+Claude3混合模型,
                2---llama3-70b-instruct)   " + "\n");
                string userInput = Console.ReadLine();
                switch (userInput)
                {

                    case "1":
                        云雀claude3混合模型_ReadLine(synthesizer);
                        break;

                    case "2":

                        llama3_70b_ReadLine(synthesizer);

                        break;

                    default:
                        云雀claude3混合模型_ReadLine(synthesizer);
                        break;

                }
                
            }
            
            Console.ReadLine();
        }


        /// <summary>
        /// 默认的模型请求方法
        /// </summary>
        /// <param name="nv"></param>
        /// <returns></returns>
        static async Task<string> Main_send(string nv)
        {
            // 设置 API 密钥（replace with your actual API key）
            //申请地址：https://build.nvidia.com/meta/llama3-70b
            string apiKey = "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";
       

            // 设置 OpenAI API 的基础 URL
            string baseUrl = "https://integrate.api.nvidia.com/v1";

            // 创建一个新的 HttpClient 实例
            HttpClient client = new HttpClient();

            // 在 Authorization 头中设置 API 密钥
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // 创建一个新的请求到 chat completions 端点
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions");


            
                   //Here is the modified code:
                  // 设置请求体为一个 JSON 对象
                  var requestBody = new
                  {
                      model = "meta/llama3-70b-instruct",
                      messages = new[]
                      {
                          new { role = "user", content = nv }
                      },
                      temperature = 0.5,
                      top_p = 1,
                      max_tokens = 1024,
                      stream = false
                  };

                   string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                   request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            
            // 发送请求并获取响应
            HttpResponseMessage response = await client.SendAsync(request);

            // 检查响应的状态代码
            if (response.IsSuccessStatusCode)
            {
                // 读取响应内容为字符串
                string responseBody = await response.Content.ReadAsStringAsync();

                // 反序列化 JSON 响应为一个 .NET 对象
                dynamic completion = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // 遍历响应的块
                foreach (var chunk in completion.choices)
                {
                    if (chunk.message.content != null)
                    {
                        return chunk.message.content;
                    }
                }
            }
            else
            {
             
                return "Error: " + response.StatusCode;
            }
            return "Error: " + "null";
        }


        public static void 云雀claude3混合模型_ReadLine(SpeechSynthesizer synthesizer)
        {
            // claude 3
            while (true)
            {

                Console.Write("我输入: ");
                string userInput = Console.ReadLine();

                if (userInput == "退出")
                {
                    break;
                }
                //申请地址：https://www.coze.cn/home
                // 设置 API 密钥（replace with your actual API key）
                var api = new CozeApiRequest("pat_Jt8M9HfcN1Eqefd024zqH3olsgKC427SrI068ygUBosbv4yDKGwcCsatFVrZ93zZ", "7373393129808019506");
                //pat_Jt8M9HfcN1Eqefd024zqH3olsgKC427SrI068ygUBosbv4yDKGwcCsatFVrZ93zZ替换为自己的API密匙
                



                var response = api.SendChatRequest("123", "123333333", userInput);
                string result = response.Result;
                // 在这里使用得到的结果

                CozeApiRequest.CozeApiResponse response_Json = JsonConvert.DeserializeObject<CozeApiRequest.CozeApiResponse>(result);

                string msg = "";
                foreach (var item in response_Json.Messages)
                {

                    if (item.Type == "answer")
                    {
                        msg = item.Content;
                        break;
                    }
                }
                Console.WriteLine($"claude 3 Opus: { msg} \n");

                // 开始语音合成并播放声音
                synthesizer.Speak(msg);

            }
        }


        public static void llama3_70b_ReadLine(SpeechSynthesizer synthesizer)
        {
            //llama3 - 70b - instruct
            while (true)
            {

                Console.Write("我输入: ");
                string userInput = Console.ReadLine();
                if (userInput == "退出")
                {

                    break;
                }
                string result = Main_send(userInput).Result;
                // 在这里使用得到的结果
                Console.WriteLine($"llama3-70b-instruct: { result} \n");

                // 开始语音合成并播放声音
                synthesizer.Speak(result);

            }
        }




    }
    

}

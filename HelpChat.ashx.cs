using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Business_App_Dev
{
    public class HelpChat : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                if (context.Request.HttpMethod != "POST")
                {
                    WriteJson(context, new { ok = false, answer = "POST only." });
                    return;
                }

                string apiKey = ConfigurationManager.AppSettings["OpenAI:ApiKey"];
                string model = ConfigurationManager.AppSettings["OpenAI:Model"] ?? "gpt-4o-mini";

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    WriteJson(context, new { ok = false, answer = "Server missing OpenAI API key (Web.config appSettings OpenAI:ApiKey)." });
                    return;
                }

                string body;
                using (var r = new StreamReader(context.Request.InputStream))
                    body = r.ReadToEnd();

                var js = new JavaScriptSerializer();
                var payload = js.Deserialize<Dictionary<string, object>>(body ?? "");
                string userMsg = payload != null && payload.ContainsKey("message") ? (payload["message"] ?? "").ToString() : "";

                if (string.IsNullOrWhiteSpace(userMsg))
                {
                    WriteJson(context, new { ok = true, answer = "Ask me something about EcoEats (orders, sellers, checkout, location, etc.)." });
                    return;
                }

                string system =
@"You are EcoEats HelpBot.
Goal: help users use the EcoEats web app (ASP.NET WebForms).
Be short, step-by-step, and practical.
If asked about orders/sellers/payment/location/account, explain how to do it in the app.
If unsure, ask 1 short follow-up question.
Do not claim you can access their database or personal data.";

                var reqObj = new
                {
                    model = model,
                    input = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = new object[] { new { type = "input_text", text = system } }
                        },
                        new
                        {
                            role = "user",
                            content = new object[] { new { type = "input_text", text = userMsg } }
                        }
                    }
                };

                string reqJson = js.Serialize(reqObj);

                string answer = CallOpenAI(apiKey, reqJson);
                WriteJson(context, new { ok = true, answer = answer });
            }
            catch
            {
                WriteJson(context, new { ok = false, answer = "Sorry—something went wrong." });
            }
        }

        private static string CallOpenAI(string apiKey, string reqJson)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var content = new StringContent(reqJson, Encoding.UTF8, "application/json");
                var resp = http.PostAsync("https://api.openai.com/v1/responses", content).Result;
                var text = resp.Content.ReadAsStringAsync().Result;

                if (!resp.IsSuccessStatusCode)
                    return "OpenAI error: " + (int)resp.StatusCode + " " + resp.ReasonPhrase;

                var js = new JavaScriptSerializer();
                var root = js.DeserializeObject(text) as Dictionary<string, object>;
                if (root == null) return "No response.";

                if (!root.ContainsKey("output")) return "No output.";

                var output = root["output"] as object[];
                if (output == null) return "No output.";

                foreach (var itemObj in output)
                {
                    var item = itemObj as Dictionary<string, object>;
                    if (item == null) continue;

                    if (!item.ContainsKey("content")) continue;
                    var contentArr = item["content"] as object[];
                    if (contentArr == null) continue;

                    foreach (var cObj in contentArr)
                    {
                        var c = cObj as Dictionary<string, object>;
                        if (c == null) continue;

                        var type = c.ContainsKey("type") ? (c["type"] ?? "").ToString() : "";
                        if (type == "output_text" && c.ContainsKey("text"))
                            return (c["text"] ?? "").ToString();
                    }
                }

                return "No text output.";
            }
        }

        private static void WriteJson(HttpContext ctx, object obj)
        {
            var js = new JavaScriptSerializer();
            ctx.Response.Write(js.Serialize(obj));
        }

        public bool IsReusable { get { return true; } }
    }
}
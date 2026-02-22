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
    public class ChatAssistant : IHttpHandler
    {
        public bool IsReusable => false;

        private static readonly HttpClient _http = new HttpClient();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                if (context.Request.HttpMethod != "POST")
                {
                    WriteJson(context, new { reply = "Use POST." }, 405);
                    return;
                }

                string body;
                using (var sr = new StreamReader(context.Request.InputStream))
                    body = sr.ReadToEnd();

                var js = new JavaScriptSerializer();
                var req = js.Deserialize<AssistantRequest>(body) ?? new AssistantRequest();

                var reply = GenerateReply(req);
                WriteJson(context, new { reply = reply }, 200);
            }
            catch
            {
                WriteJson(context, new { reply = "Server error." }, 500);
            }
        }

        private string GenerateReply(AssistantRequest req)
        {
            var apiKey = ConfigurationManager.AppSettings["OpenAI_ApiKey"];
            var endpoint = ConfigurationManager.AppSettings["OpenAI_Endpoint"] ?? "https://api.openai.com/v1/chat/completions";
            var model = ConfigurationManager.AppSettings["OpenAI_Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("PASTE_"))
            {
                // fallback if key not set (still returns something, avoids "network error")
                return RuleBasedFallback(req);
            }

            // Build a compact items list
            var sb = new StringBuilder();
            sb.AppendLine("User preference: " + (req.message ?? ""));
            sb.AppendLine("Items currently on screen:");
            if (req.items != null)
            {
                int i = 0;
                foreach (var it in req.items)
                {
                    if (++i > 18) break;
                    sb.AppendLine($"- {it.name} | {it.category} | ${it.price} | {it.distanceKm}km | {it.store}");
                }
            }

            var system = "You are EcoEats Assistant. Recommend ONLY using the items provided. " +
                         "Pick 1-3 best matches. Explain briefly. If none match, say so and suggest closest alternatives.";

            var payload = new
            {
                model = model,
                temperature = 0.4,
                messages = new object[]
                {
                    new { role = "system", content = system },
                    new { role = "user", content = sb.ToString() }
                }
            };

            var json = new JavaScriptSerializer().Serialize(payload);

            using (var msg = new HttpRequestMessage(HttpMethod.Post, endpoint))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = _http.SendAsync(msg).GetAwaiter().GetResult();
                var respText = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!resp.IsSuccessStatusCode)
                {
                    return "Network/server error. Try again.";
                }

                // Parse: choices[0].message.content
                var root = new JavaScriptSerializer().DeserializeObject(respText) as Dictionary<string, object>;
                if (root == null || !root.ContainsKey("choices")) return RuleBasedFallback(req);

                var choices = root["choices"] as object[];
                if (choices == null || choices.Length == 0) return RuleBasedFallback(req);

                var c0 = choices[0] as Dictionary<string, object>;
                var message = c0?["message"] as Dictionary<string, object>;
                var content = message?["content"] as string;

                if (string.IsNullOrWhiteSpace(content)) return RuleBasedFallback(req);
                return content.Trim();
            }
        }

        private string RuleBasedFallback(AssistantRequest req)
        {
            // Simple fallback (works even with no API key)
            var pref = (req.message ?? "").ToLowerInvariant();
            if (req.items == null || req.items.Count == 0) return "I can’t see any items on your screen right now.";

            Item best = req.items[0];

            foreach (var it in req.items)
            {
                var name = (it.name ?? "").ToLowerInvariant();
                var cat = (it.category ?? "").ToLowerInvariant();

                bool sweet = pref.Contains("sweet");
                bool spicy = pref.Contains("spicy");
                bool budget = pref.Contains("budget") || pref.Contains("cheap");

                double bestScore = Score(best, sweet, spicy, budget);
                double itScore = Score(it, sweet, spicy, budget);

                if (itScore > bestScore) best = it;
            }

            return $"Try: {best.name} ({best.category}) from {best.store}. Price: ${best.price}, Distance: {best.distanceKm}km.";
        }

        private double Score(Item it, bool sweet, bool spicy, bool budget)
        {
            double score = 0;
            var name = (it.name ?? "").ToLowerInvariant();
            var cat = (it.category ?? "").ToLowerInvariant();

            if (sweet && (name.Contains("tea") || name.Contains("milk") || cat.Contains("dessert") || cat.Contains("drinks"))) score += 3;
            if (spicy && (name.Contains("spicy") || name.Contains("curry") || cat.Contains("asian"))) score += 3;

            double price = 999;
            double.TryParse((it.price ?? "").ToString(), out price);
            if (budget) score += Math.Max(0, 6 - price); // cheaper => higher

            double dist = 99;
            double.TryParse((it.distanceKm ?? "").ToString(), out dist);
            score += Math.Max(0, 4 - dist);

            return score;
        }

        private void WriteJson(HttpContext ctx, object obj, int status)
        {
            ctx.Response.StatusCode = status;
            var json = new JavaScriptSerializer().Serialize(obj);
            ctx.Response.Write(json);
        }

        public class AssistantRequest
        {
            public string message { get; set; }
            public List<Item> items { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string name { get; set; }
            public string category { get; set; }
            public string price { get; set; }
            public string distanceKm { get; set; }
            public string store { get; set; }
        }
    }
}
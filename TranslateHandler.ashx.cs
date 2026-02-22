using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Business_App_Dev.Services;

namespace Business_App_Dev
{
    public class TranslateHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                string body;
                using (var sr = new StreamReader(context.Request.InputStream))
                    body = sr.ReadToEnd();

                var js = new JavaScriptSerializer();
                var req = js.Deserialize<TranslateRequest>(body) ?? new TranslateRequest();

                string target = (req.targetLang ?? "en").ToLowerInvariant();
                var texts = (req.texts ?? new List<string>())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();

                if (target == "en" || texts.Count == 0)
                {
                    context.Response.Write(js.Serialize(new TranslateResponse { translations = texts }));
                    return;
                }

                var result = new List<string>();

                foreach (var t in texts)
                {
                    string translated = TranslationCache.GetOrAdd(
                        $"domtx:{target}:{t}",
                        () => TranslationService.Translate(t, target, "en"),
                        24);

                    result.Add(translated);
                }

                context.Response.Write(js.Serialize(new TranslateResponse { translations = result }));
            }
            catch
            {
                context.Response.Write("{\"translations\":[]}");
            }
        }

        class TranslateRequest
        {
            public string targetLang { get; set; }
            public List<string> texts { get; set; }
        }

        class TranslateResponse
        {
            public List<string> translations { get; set; }
        }
    }
}
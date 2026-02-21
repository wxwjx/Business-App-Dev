using System;
using System.Configuration;
using Google.Cloud.Translation.V2;

namespace Business_App_Dev.Services
{
    public static class TranslationService
    {
        private static readonly string ApiKey =
            ConfigurationManager.AppSettings["GoogleTranslateApiKey"];

        public static string Translate(
            string text,
            string targetLang,
            string sourceLang = "en")
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            if (string.IsNullOrWhiteSpace(targetLang) ||
                targetLang.Equals(sourceLang, StringComparison.OrdinalIgnoreCase))
                return text;

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new Exception("GoogleTranslateApiKey not found in web.config.");

            // Create official Google client
            var client = TranslationClient.CreateFromApiKey(ApiKey);

            var result = client.TranslateText(
                text,
                targetLang,
                sourceLang
            );

            return result.TranslatedText;
        }
    }
}

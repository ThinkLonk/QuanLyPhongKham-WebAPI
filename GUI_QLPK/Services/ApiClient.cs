using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace GUI_QLPK
{
    internal static class ApiClient
    {
        private static readonly HttpClient Client = CreateClient();
        private static DateTime retryAfter = DateTime.MinValue;

        private static HttpClient CreateClient()
        {
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "http://localhost:5000/";
            }

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(1)
            };
        }

        private static bool IsEnabled
        {
            get
            {
                string enabled = ConfigurationManager.AppSettings["ApiEnabled"];
                return !string.Equals(enabled, "false", StringComparison.OrdinalIgnoreCase) &&
                       DateTime.Now >= retryAfter;
            }
        }

        public static bool TryGet<T>(string route, out T value)
        {
            value = default(T);
            if (!IsEnabled) return false;

            try
            {
                HttpResponseMessage response = Client.GetAsync(route).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return false;

                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                value = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                MarkUnavailable();
                return false;
            }
        }

        public static bool TryPost<TRequest, TResponse>(string route, TRequest body, out TResponse value)
        {
            value = default(TResponse);
            if (!IsEnabled) return false;

            try
            {
                HttpResponseMessage response = Client.PostAsync(route, ToJsonContent(body)).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return false;

                string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                value = string.IsNullOrWhiteSpace(json) ? default(TResponse) : JsonConvert.DeserializeObject<TResponse>(json);
                return true;
            }
            catch
            {
                MarkUnavailable();
                return false;
            }
        }

        public static bool TryPut<TRequest>(string route, TRequest body)
        {
            if (!IsEnabled) return false;

            try
            {
                HttpResponseMessage response = Client.PutAsync(route, ToJsonContent(body)).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                MarkUnavailable();
                return false;
            }
        }

        public static bool TryDelete(string route)
        {
            if (!IsEnabled) return false;

            try
            {
                HttpResponseMessage response = Client.DeleteAsync(route).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                MarkUnavailable();
                return false;
            }
        }

        private static void MarkUnavailable()
        {
            retryAfter = DateTime.Now.AddSeconds(20);
        }

        private static StringContent ToJsonContent<T>(T body)
        {
            string json = JsonConvert.SerializeObject(body);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}

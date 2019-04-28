using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NixonWilliamsScraper
{
    public class VantageGetter : IPageGetter
    {
        private Uri baseUri = new Uri("https://www.nixonwilliamsvantage.com");
        private CookieContainer cookies = new CookieContainer();
        private HttpClient client;

        public VantageGetter()
        {
            client = new HttpClient(new HttpClientHandler { CookieContainer = cookies })
            {
                BaseAddress = baseUri
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
        }

        public async Task<Stream> Get(string path)
        {
            var response = await client.GetAsync(path);
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task Login(string username, string password)
        {
            var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "btn_submit", "Login" }
            });

            var response = await client.PostAsync("/system/login", request);

            Console.WriteLine($"login: {response.StatusCode}");
        }
    }


}

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Application;
using Application.User;
using Microsoft.Extensions.Options;

namespace Infrastructure.SocialLogins
{
    public class FacebookGraphAPIAccessor : IFacebookGraphAPIAccessor
    {
        private readonly IOptions<FacebookAppSettings> _options;
        private readonly HttpClient _client;

        public FacebookGraphAPIAccessor(IOptions<FacebookAppSettings> options)
        {
            _options = options;
            _client = new HttpClient
            {
                BaseAddress = new System.Uri("https://graph.facebook.com")
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public async Task<FacebookUserInfo> FacebookLogin(string accessToken)
        {
            var verifyToken =
                await  _client.GetAsync(
                    $"debug_token?input_token={accessToken}&access_token={_options.Value.AppId}|{_options.Value.AppSecret}");

            if (!verifyToken.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await GetAsync<FacebookUserInfo>(accessToken,
                "fields=name,email,birthday,picture.width(100).height(100)");
            return result;
        }

        private async Task<T> GetAsync<T>(string accessToken, string args)
        {
            var response = await _client.GetAsync($"me?access_token={accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var result = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return await JsonSerializer.DeserializeAsync<T>(result, options);
        }
    }
}
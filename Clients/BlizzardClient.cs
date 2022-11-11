using Microsoft.Extensions.Configuration;
using Shinra.Clients.Models;
using Shinra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public class BlizzardClient : IBlizzardClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public BlizzardClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<CharacterStatistics> GetCharacterStatistics(string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://us.api.blizzard.com/profile/wow/character/{realm}/{characterName}/achievements/statistics?namespace=profile-us&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterStatistics>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterProfile> GetCharacterProfile(string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://us.api.blizzard.com/profile/wow/character/{realm}/{characterName}?namespace=profile-us&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterProfile>(await response.Content.ReadAsStringAsync());
        }

        public async Task<string> GetAccessToken()
        {
            string clientId = _config["ClientID"];
            string clientSecret = _config["ClientSecret"];
            string credentials = $"{clientId}:{clientSecret}";
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

            List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "wow.profile")
            };
            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

            var request = await _httpClient.PostAsync("https://oauth.battle.net/token", requestBody);
            var response = await request.Content.ReadAsStringAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return JsonSerializer.Deserialize<AccessToken>(response).access_token;
        }
    }

    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }
}

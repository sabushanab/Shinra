﻿using Microsoft.AspNetCore.Mvc;
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

        [ResponseCache(Duration = 180, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "region" })]
        public async Task<RealmContainer> GetRealms(string region)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/data/wow/realm/index?namespace=dynamic-{region}&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<RealmContainer>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterStatistics> GetCharacterStatistics(string region, string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}/achievements/statistics?namespace=profile-{region}&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterStatistics>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterAchievements> GetCharacterAchievements(string region, string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}/achievements?namespace=profile-{region}&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterAchievements>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterMythicPlusScore> GetMythicPlusScore(string region, string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}/mythic-keystone-profile?namespace=profile-{region}&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterMythicPlusScore>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterMythicPlusSeasonDetails> GetMythicPlusSeasonDetails(string region, string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}/mythic-keystone-profile/season/10?namespace=profile-{region}&locale=en_us&access_token={accessToken}");

            var response = await _httpClient.SendAsync(request);
            return JsonSerializer.Deserialize<CharacterMythicPlusSeasonDetails>(await response.Content.ReadAsStringAsync());
        }

        public async Task<CharacterProfile> GetCharacterProfile(string region, string realm, string characterName)
        {
            var accessToken = await CacheService.GetAndSetAsync("blizzard_access_token", () => GetAccessToken());
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}?namespace=profile-{region}&locale=en_us&access_token={accessToken}");

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

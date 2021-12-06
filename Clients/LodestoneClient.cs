using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public class LodestoneClient : ILodestoneClient
    {
        private readonly HttpClient _httpClient;
        public LodestoneClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HtmlDocument> GetCharacter(long characterID)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://na.finalfantasyxiv.com/lodestone/character/{characterID}/");

            var response = await _httpClient.SendAsync(request);
            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            return pageDocument;
        }

        public async Task<HtmlDocument> GetCharacterClassJob(long characterID)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://na.finalfantasyxiv.com/lodestone/character/{characterID}/class_job/");

            var response = await _httpClient.SendAsync(request);
            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            return pageDocument;
        }

        public async Task<HtmlDocument> GetCharacterMounts(long characterID)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://na.finalfantasyxiv.com/lodestone/character/{characterID}/mount/");
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU OS 10_15_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Mobile/14E304 Safari/605.1.15");
            var response = await _httpClient.SendAsync(request);
            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            return pageDocument;
        }

        public async Task<HtmlDocument> GetCharacterMinions(long characterID)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://na.finalfantasyxiv.com/lodestone/character/{characterID}/minion/");
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (iPhone; CPU OS 10_15_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1.1 Mobile/14E304 Safari/605.1.15");
            var response = await _httpClient.SendAsync(request);
            string pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);

            return pageDocument;
        }
    }
}

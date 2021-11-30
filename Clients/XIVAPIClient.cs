using Akka.Actor;
using Newtonsoft.Json;
using Shinra.Actors;
using Shinra.Messages.Character;
using Shinra.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shinra.Clients
{
    public class XIVAPIClient : IXIVAPIClient
    {
        private readonly HttpClient _httpClient;
        public XIVAPIClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<FreeCompanyMembersContainer> GetFreeCompanyMembers() 
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://xivapi.com/freecompany/9230971861226067551?data=FCM");

            var response = await _httpClient.SendAsync(request);
            FreeCompanyMembersContainer freeCompanyMembers = null;
            if (response.IsSuccessStatusCode) 
            {
                freeCompanyMembers = JsonConvert.DeserializeObject<FreeCompanyMembersContainer>(await response.Content.ReadAsStringAsync());
                CacheService.Set(nameof(XIVAPIClient.GetFreeCompanyMembers), freeCompanyMembers, 20);
            }
            return freeCompanyMembers;
        }

        public async Task GetEachFreeCompanyMember() 
        {
            FreeCompanyMembersContainer freeCompanyMembers = await GetFreeCompanyMembers();
            foreach(var member in freeCompanyMembers.FreeCompanyMembers) 
            {
                ActorService.CharacterSupervisor.Tell(new UpdateCharacterMessage(member.ID));
            }
        }

        public async Task<CharacterContainer> GetCharacter(int id) 
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://xivapi.com/character/{id}?data=mimo");
            var response = await _httpClient.SendAsync(request);
            CharacterContainer character = null;
            if (response.IsSuccessStatusCode) 
            {
                try 
                {
                    character = JsonConvert.DeserializeObject<CharacterContainer>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings 
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    CacheService.Set(id.ToString(), character, 60);
                } 
                catch 
                {
                    //log here
                }
            }
            return character;
        }
    }
}
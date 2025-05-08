using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Client.Contracts;
using Client.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Share.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Service
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public ArticleRepository(HttpClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> AddGroup(GroupModel group)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PostAsJsonAsync(Endpoints.AddGroupEndpoint, group);
            return response.IsSuccessStatusCode;
        }

        public async Task<IList<GroupModel>> GetAllGroup(string email)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<IList<GroupModel>>(Endpoints.GetAllGroupsEndpoint + email);
            return reponse;
        }

        private async Task<string> GetBearerToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

    }
}

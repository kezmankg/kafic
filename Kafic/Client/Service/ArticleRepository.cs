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

        public async Task<GroupModel> GetGroupById(string id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<GroupModel>(Endpoints.GetGroupByIdEndpoint + id);
            return reponse;
        }

        public async Task<bool> UpdateGroup(GroupModel model)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PutAsJsonAsync(Endpoints.UpdateGroupEndpoint, model);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> AddSubGroup(SubgroupModel group)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PostAsJsonAsync(Endpoints.AddSubGroupEndpoint, group);
            return response.IsSuccessStatusCode;
        }

        public async Task<IList<GroupModel>> GetAllGroupWithSubgroup(string email)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<IList<GroupModel>>(Endpoints.GetGroupByIdWithSubgroupEndpoint + email);
            return reponse;
        }

        public async Task<SubgroupModel> GetSubGroupById(string id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<SubgroupModel>(Endpoints.GetSubGroupByIdEndpoint + id);
            return reponse;
        }
        public async Task<bool> SubUpdateGroup(SubgroupModel model)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PutAsJsonAsync(Endpoints.UpdateSubGroupEndpoint, model);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddArticle(ArticleModel group)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PostAsJsonAsync(Endpoints.AddArticleEndpoint, group);
            return response.IsSuccessStatusCode;
        }

        public async Task<IList<GroupModel>> GetAllArticles(string email)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<IList<GroupModel>>(Endpoints.GetAllArticlesEndpoint + email);
            return reponse;
        }
        private async Task<string> GetBearerToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

    }
}

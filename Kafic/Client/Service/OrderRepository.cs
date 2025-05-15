using Blazored.LocalStorage;
using Client.Contracts;
using Client.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Share.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Service
{
    public class OrderRepository : IOrderRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public OrderRepository(HttpClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> AddOrder(OrderModel model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PostAsJsonAsync(Endpoints.AddOrderEndpoint, model);
            return response.IsSuccessStatusCode;
        }

        public async Task<IList<OrderModel>> GetAllOrder(string email, string deskno)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<IList<OrderModel>>(Endpoints.GetAllOrdersEndpoint + email + "/" + deskno);
            return reponse;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            if (id < 1)
                return false;

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.DeleteAsync(Endpoints.DeleteGroupEndpoint + id);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            return false;
        }

        public async Task<bool> DeleteArticle(int idOrder, int idArticle)
        {
            if (idOrder < 1 || idArticle < 1)
                return false;

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.DeleteAsync(Endpoints.DeleteGroupEndpoint + idOrder + "/" + idArticle);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            return false;
        }

        public async Task<bool> PayOrder(string descNo, string userEmail)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            PayOrderModel model = new PayOrderModel();
            model.DescNo = descNo;
            model.UserEmail = userEmail;
            var response = await _client.PostAsJsonAsync(Endpoints.AddOrderEndpoint, model);
            return response.IsSuccessStatusCode;
        }

        private async Task<string> GetBearerToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}

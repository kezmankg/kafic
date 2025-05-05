using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Client.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Share.Models;
using System.Net.Http.Json;
using System.Net;
using Client.Static;

namespace Client.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ISessionStorageService _sessionStorageService;

        public AuthenticationRepository(HttpClient client,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            ISessionStorageService sessionStorageService)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _sessionStorageService = sessionStorageService;
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.RegisterEndpoint
                , user);
            return response.IsSuccessStatusCode;
        }
    }
}

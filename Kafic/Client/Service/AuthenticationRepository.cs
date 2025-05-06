using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Client.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Share.Models;
using System.Net.Http.Json;
using System.Net;
using Client.Static;
using Client.Providers;
using Newtonsoft.Json;
using System.Net.Http.Headers;

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

        public async Task<string?> Register(RegistrationModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.RegisterEndpoint, user);

            if (response.IsSuccessStatusCode)
            {
                return null; // Sve prošlo kako treba
            }

            // Pročitaj tekst greške iz responsa
            var errorContent = await response.Content.ReadAsStringAsync();
            return errorContent;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.LoginEndpoint, user);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            //Store Token
            await _localStorage.SetItemAsync("authToken", token.Token);

            //Change auth state of app
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                .LoggedIn();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider)
                .LoggedOut();
            await _sessionStorageService.ClearAsync();
        }
    }
}

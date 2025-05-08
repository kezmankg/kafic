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

        public async Task<CompanyModel> GetCompanyPerEmail(string email)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<CompanyModel>(Endpoints.GetCompanyEndpoint + email);
            return reponse;
        }

        public async Task<bool> UpdateCompany(CompanyModel companyModel)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PutAsJsonAsync(Endpoints.UpdateCompanyEndpoint, companyModel);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> RegisterUser(RegistrationUserModel user)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PostAsJsonAsync(Endpoints.RegisterUserEndpoint, user);

            if (response.IsSuccessStatusCode)
            {
                return null; // Sve prošlo kako treba
            }

            // Pročitaj tekst greške iz responsa
            var errorContent = await response.Content.ReadAsStringAsync();
            return errorContent;
        }

        public async Task<IList<RegistrationUserModel>> GetAllUsers(string email)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<IList<RegistrationUserModel>>(Endpoints.GetAllUsersEndpoint + email);
            return reponse;
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.DeleteAsync(Endpoints.DeleteUserEndpoint + id.ToString());

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
                
            return false;

        }

        public async Task<RegistrationUserModelEdit> GetUserPerId(string id)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", await GetBearerToken());

            var reponse = await _client.GetFromJsonAsync<RegistrationUserModelEdit>(Endpoints.GetUserByIdEndpoint + id);
            return reponse;
        }

        public async Task<bool> UpdateUser(RegistrationUserModelEdit model)
        {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("bearer", await GetBearerToken());
            var response = await _client.PutAsJsonAsync(Endpoints.UpdateUserEndpoint, model);
            return response.IsSuccessStatusCode;
        }

        private async Task<string> GetBearerToken()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}

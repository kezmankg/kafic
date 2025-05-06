using Share.Models;

namespace Client.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<string?> Register(RegistrationModel user);
        public Task<bool> Login(LoginModel user);
    }
}

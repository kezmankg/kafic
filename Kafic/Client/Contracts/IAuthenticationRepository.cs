using Share.Models;

namespace Client.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<string?> Register(RegistrationModel user);
        public Task<bool> Login(LoginModel user);
        public Task Logout();
        public Task<CompanyModel> GetCompanyPerEmail(string email);
        public Task<bool> UpdateCompany(CompanyModel companyModel);
        public Task<string?> RegisterUser(RegistrationUserModel user);
        public Task<IList<RegistrationUserModel>> GetAllUsers(string email);
    }
}

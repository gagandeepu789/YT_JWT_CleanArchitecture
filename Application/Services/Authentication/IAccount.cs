using Application.DTOs;
namespace Application.Services.Authentication
{
    public interface IAccount
    {
        Task<RegistrationResponse> RegisterAccountAsync(RegisterUserDTO model);
        Task<LoginResponse> LoginAccountAsyc(LoginDTO model);
    }
}

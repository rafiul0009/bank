using PayBill.UserManagement.Models;

namespace PayBill.UserManagement.Repositories.Infrastructure;

public interface IAuthRepository
{
    Task<RegisterResponseModel> RegisterAsync(UserInfoModel userInfo);
    Task<UserInfoModel> LoginAsync(UserLoginModel userLogin);
}
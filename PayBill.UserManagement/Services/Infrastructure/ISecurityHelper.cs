using PayBill.UserManagement.Models;

namespace PayBill.UserManagement.Services.Infrastructure;

public interface ISecurityHelper
{
    Task<string> GenerateJsonWebTokenAsync(UserInfoModel userInfo);
    bool IsValidHash(string senderHash, string payLoad = "Default Payload");
}
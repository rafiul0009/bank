using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PayBill.UserManagement.Models;
using PayBill.UserManagement.Services.Infrastructure;

namespace PayBill.UserManagement.Services.Persistence;

public class SecurityHelper : ISecurityHelper
{
    private readonly IConfiguration _config;

    public SecurityHelper(IConfiguration config)
    {
        _config = config;
    }

    #region "Generate JSON Web Token"

    public async Task<string> GenerateJsonWebTokenAsync(UserInfoModel userInfo)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
            new Claim(JwtRegisteredClaimNames.Name, userInfo.UserName),
            new Claim(JwtRegisteredClaimNames.GivenName, userInfo.Name),
            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            _config["JWT:Issuer"],
            _config["JWT:Audience"],
            claims,
            DateTime.UtcNow, // When this token becomes valid
            DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["JWT:Expires"])), // When token will expire
            credentials
        );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    #endregion

    public bool IsValidHash(string senderHash, string payLoad = "Default Payload")
    {
        return senderHash == GenerateHash(payLoad);
    }

    #region "Hash Generator"

    public string GenerateHash(string payload = "Default Payload")
    {
        using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_config["Hash:HashKey"]!)))
        {
            var data = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(data);
        }
    }

    #endregion
}
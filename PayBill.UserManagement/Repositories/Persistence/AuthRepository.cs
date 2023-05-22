using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PayBill.UserManagement.Identity;
using PayBill.UserManagement.Models;
using PayBill.UserManagement.Repositories.Infrastructure;

namespace PayBill.UserManagement.Repositories.Persistence;

public class AuthRepository : IAuthRepository
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #region "Register"

    public async Task<RegisterResponseModel> RegisterAsync(UserInfoModel userInfo)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = userInfo.UserName,
            FullName = userInfo.Name,
            Email = userInfo.Email,
            PhoneNumber = userInfo.PhoneNumber
        };

        var result = await _userManager.CreateAsync(applicationUser, userInfo.Password);

        if (result.Succeeded)
            await _userManager.AddClaimAsync(applicationUser, new Claim("FullName", userInfo.Name));

        return new RegisterResponseModel
        {
            RequireConfirmedAccount = true,
            SignedIn = false
        };
    }

    #endregion

    #region "Login"

    public async Task<UserInfoModel> LoginAsync(UserLoginModel userLogin)
    {
        var result = await _signInManager.PasswordSignInAsync(userLogin.UserName, userLogin.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(userLogin.UserName);
            if (user != null)
                return new UserInfoModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.FullName,
                    Email = user.Email
                };
        }

        return null;
    }

    #endregion
}
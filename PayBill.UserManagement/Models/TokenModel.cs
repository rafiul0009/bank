namespace PayBill.UserManagement.Models;

public class TokenModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string JwtToken { get; set; }
    public DateTime Expires { get; set; }
}
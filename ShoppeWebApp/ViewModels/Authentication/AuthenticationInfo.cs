using System.Security.Claims;

namespace ShoppeWebApp.ViewModels.Authentication
{
    public class AuthenticationInfo
    {
        public static ClaimsIdentity CreateCustomerIdentity(string id, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Customer")
            };
            return new ClaimsIdentity(claims, "CustomerIdentity");
        }
        public static ClaimsIdentity CreateSellerIdentity(string id, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Seller")
            };
            return new ClaimsIdentity(claims, "SellerIdentity");
        }
        public static ClaimsIdentity CreateAdminIdentity(string id, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };
            return new ClaimsIdentity(claims, "AdminIdentity");
        }
    }
}

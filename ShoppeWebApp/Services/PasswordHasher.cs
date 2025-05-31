using System.Text;
using System.Security.Cryptography;

namespace ShoppeWebApp.Services
{
public class PasswordHasher
{
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
    }
}
}

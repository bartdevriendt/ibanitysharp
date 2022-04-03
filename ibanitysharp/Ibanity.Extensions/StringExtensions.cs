using System.Security.Cryptography;

namespace Ibanity.Extensions;

public static class StringExtensions
{

    public static string ToBase64Sha512(this string toEncode)
    {
        var data = System.Text.Encoding.UTF8.GetBytes(toEncode);
        SHA512 sha = new SHA512Managed();
        var result = sha.ComputeHash(data);
        return System.Convert.ToBase64String(result);
    }
    
}
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Ibanity.Utils;

public class Signing
{
    
    private static readonly Encoding Encoding = Encoding.UTF8;
    public static string GenerateSignatureForData(string data, X509Certificate2 signingKey)
    {

        var bytes = Encoding.GetBytes(data);
        
        byte[] signature;
        using (var privateKey = signingKey.GetRSAPrivateKey())
            signature = privateKey.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        return Convert.ToBase64String(signature);
    }
}
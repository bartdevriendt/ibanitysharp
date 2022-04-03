using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Ibanity.Utils;

public class Signing
{
    public static string GenerateSignatureForData(string data, X509Certificate2 signingKey)
    {
        var bcCert = TransformRSAPrivateKey(signingKey.PrivateKey);
        var keyLen = (int)Math.Ceiling((signingKey.GetRSAPrivateKey().KeySize - 1) / 8.0);

        byte[] signedBytes = CreateSignature(Encoding.ASCII.GetBytes(data), bcCert, keyLen);
        return Convert.ToBase64String(signedBytes);
    }
    
    private static AsymmetricKeyParameter TransformRSAPrivateKey(AsymmetricAlgorithm privateKey)
    {
        RSACryptoServiceProvider prov = privateKey as RSACryptoServiceProvider;
        RSAParameters parameters = prov.ExportParameters(true);

        return new RsaPrivateCrtKeyParameters(
            new BigInteger(1, parameters.Modulus),
            new BigInteger(1, parameters.Exponent),
            new BigInteger(1, parameters.D),
            new BigInteger(1, parameters.P),
            new BigInteger(1, parameters.Q),
            new BigInteger(1, parameters.DP),
            new BigInteger(1, parameters.DQ),
            new BigInteger(1, parameters.InverseQ));
    }

    private static byte[] CreateSignature(byte[] data, AsymmetricKeyParameter privateKey, int keyLength)
    {
        var digest = new Sha256Digest();
        var saltLength = 32; //keyLength - digest.GetDigestSize() - 2;

        PssSigner signer = new PssSigner(new RsaEngine(), new Sha256Digest(), digest, saltLength);
        signer.Init(true, new ParametersWithRandom((RsaPrivateCrtKeyParameters)privateKey));
        signer.BlockUpdate(data, 0, data.Length);
        return signer.GenerateSignature();
    }
}
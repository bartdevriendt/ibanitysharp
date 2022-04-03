using System.Security.Cryptography.X509Certificates;

namespace Ibanity.Certificates;

/// <summary>
/// Provides an interface to load certificates from any source
/// </summary>
public interface ICertificateStore
{
    X509Certificate2 GetAccessCertificate();
    X509Certificate2 GetSigningCertificate();
}
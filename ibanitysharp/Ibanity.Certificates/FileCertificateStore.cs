using System.Security.Cryptography.X509Certificates;

namespace Ibanity.Certificates;

/// <summary>
/// Default implementation of a certificate store. It allows loading the certiifcate from a .pfx file
/// </summary>
public class FileCertificateStore : ICertificateStore
{

    private string _accessCertificateFileName;
    private string _signingCertificateFileName;
    private string _password;
    
    public FileCertificateStore(string accessCertificate, string password, string signingCertificateFileName)
    {
        _accessCertificateFileName = accessCertificate;
        _password = password;
        _signingCertificateFileName = signingCertificateFileName;
    }
    public X509Certificate2 GetAccessCertificate()
    {
        X509Certificate2 x509 = new X509Certificate2(_accessCertificateFileName, _password);
        return x509;
    }

    public X509Certificate2 GetSigningCertificate()
    {
        X509Certificate2 x509 = new X509Certificate2(_signingCertificateFileName, _password, X509KeyStorageFlags.Exportable);
        return x509;
    }
}
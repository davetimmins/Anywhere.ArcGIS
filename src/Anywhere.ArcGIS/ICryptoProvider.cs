using Anywhere.ArcGIS.Operation;

namespace Anywhere.ArcGIS
{
    public interface ICryptoProvider
    {
        GenerateToken Encrypt(GenerateToken tokenRequest, byte[] exponent, byte[] modulus);
    }
}

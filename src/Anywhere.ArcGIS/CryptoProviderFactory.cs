using System;
using System.Text;

namespace Anywhere.ArcGIS
{
    public static class CryptoProviderFactory
    {
        public static Func<ICryptoProvider> Get { get; set; }

        public static bool Enabled { get; set; }

        static CryptoProviderFactory()
        {
            Get = (() => { return Enabled ? new RsaEncrypter() : null; });
        }
    }

    public class RsaEncrypter : ICryptoProvider
    {
        public Operation.GenerateToken Encrypt(Operation.GenerateToken tokenRequest, byte[] exponent, byte[] modulus)
        {
            if (tokenRequest == null)
            {
                throw new ArgumentNullException(nameof(tokenRequest));
            }

            if (exponent == null)
            {
                throw new ArgumentNullException(nameof(exponent));
            }

            if (modulus == null)
            {
                throw new ArgumentNullException(nameof(modulus));
            }

            if (tokenRequest.Encrypted == true)
            {
                return tokenRequest;
            }

            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(512))
            {
                var rsaParms = new System.Security.Cryptography.RSAParameters
                {
                    Exponent = exponent,
                    Modulus = modulus
                };
                rsa.ImportParameters(rsaParms);

                var encryptedUsername = rsa.Encrypt(Encoding.UTF8.GetBytes(tokenRequest.Username), false).BytesToHex();
                var encryptedPassword = rsa.Encrypt(Encoding.UTF8.GetBytes(tokenRequest.Password), false).BytesToHex();
                var encryptedClient = string.IsNullOrWhiteSpace(tokenRequest.Client) ? "" : rsa.Encrypt(Encoding.UTF8.GetBytes(tokenRequest.Client), false).BytesToHex();
                var encryptedExpiration = rsa.Encrypt(Encoding.UTF8.GetBytes(tokenRequest.ExpirationInMinutes.ToString()), false).BytesToHex();
                var encryptedReferer = string.IsNullOrWhiteSpace(tokenRequest.Referer) ? "" : rsa.Encrypt(Encoding.UTF8.GetBytes(tokenRequest.Referer), false).BytesToHex();

                tokenRequest.Encrypt(encryptedUsername, encryptedPassword, encryptedExpiration, encryptedClient, encryptedReferer);

                return tokenRequest;
            }
        }
    }
}

using System.Text;

namespace Anywhere.ArcGIS
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Hex-encodes a byte array.
        /// </summary>
        /// <param name="bytes">Byte array to encode</param>
        /// <returns>Hex-encoded string</returns>
        public static string BytesToHex(this byte[] bytes)
        {
            if (bytes == null) return string.Empty;

            var sb = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", bytes[i]);
            }

            return sb.ToString();
        }
    }
}

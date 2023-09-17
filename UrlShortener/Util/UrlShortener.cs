using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UrlShortener.Util
{
    public class UrlShortener
    {
        private const string Salt = "HelloC";
        private const string Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string ShortenUrl(string? longUrl)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Add the unique salt to the long URL
                string saltedUrl = longUrl + Salt;

                byte[] inputBytes = Encoding.UTF8.GetBytes(saltedUrl);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the hash to a base62-encoded string
                string base62Hash = Base62Encode(hashBytes);

                // Ensure the output is always 6 characters long
                if (base62Hash.Length < 6)
                {
                    string outputValue = string.Format("{0:D4}", base62Hash);
                    base62Hash = outputValue.PadLeft(6, '0');
                }
                else if (base62Hash.Length > 6)
                {
                    base62Hash = base62Hash.Substring(0, 6);
                }

                return base62Hash;
            }
        }

        private string Base62Encode(byte[] bytes)
        {
            BigInteger value = new BigInteger(bytes.Reverse().ToArray()); // Convert bytes to a positive BigInteger
            if(value < 0)
            {
                byte[] temp = new byte[bytes.Length];
                Array.Copy(bytes, temp, bytes.Length);
                bytes = new byte[temp.Length + 1];
                Array.Copy(temp, bytes, temp.Length);
                value = new BigInteger(bytes);
            }

            StringBuilder result = new StringBuilder();

            while (value > 0)
            {
                int index = (int)(value % 62); // Calculate the index for the Base62 character
                result.Insert(0, Base62Characters[index]);
                value /= 62; // Divide by 62 to move to the next digit
            }

            return result.ToString();
        }

        public string CombineUrl(string originalUrl, string shortUrlCode)
        {
            var uri = new Uri(originalUrl);
            var host = uri.Host;
            return host + "/" + shortUrlCode;
        }

    }
}

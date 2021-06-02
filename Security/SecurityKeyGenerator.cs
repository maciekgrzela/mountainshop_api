using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Security
{
    public class SecurityKeyGenerator
    {
        private static SecurityKeyGenerator instance = null;
        private static SymmetricSecurityKey key = null;

        public static SecurityKeyGenerator Instance
        {
            get
            {
                return instance ??= new SecurityKeyGenerator();
            }
        }

        private SecurityKeyGenerator()
        {
            TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();
            tripleDes.GenerateKey();
            key = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(tripleDes.Key.ToString()));
        }

        public SymmetricSecurityKey GetKey()
        {
            return key;
        }
    }
}
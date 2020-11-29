using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;

namespace Transaction.Core
{

    class RngAlphaNumericStringGenerator : IRandomStringGenerator
    {
        static readonly char[] AvailableCharacters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        static readonly string CapitalSmallNumbers_ = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-abcdefghijklmnopqorstuvwxyz_0123456789";

        public string GenerateIdentifier(int length, CharsInToken charsInToken)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            if (charsInToken == CharsInToken.CapitalNumeric)
            {
                for (int idx = 0; idx < identifier.Length; idx++)
                {
                    int pos = randomData[idx] % AvailableCharacters.Length;
                    identifier[idx] = AvailableCharacters[pos];
                }
                return new string(identifier);
            }
            else if (charsInToken == CharsInToken.CapitalSmallNumeric_)
            {
                for (int idx = 0; idx < identifier.Length; idx++)
                {
                    int pos = randomData[idx] % CapitalSmallNumbers_.Length;
                    identifier[idx] = CapitalSmallNumbers_[pos];
                }
                return new string(identifier);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

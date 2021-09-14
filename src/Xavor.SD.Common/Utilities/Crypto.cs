using System;
using System.Collections.Generic;
using System.Text;

namespace Xavor.SD.Common.Utilities
{
    public static class Crypto
    {
        public static string Encrypt(this string text)
        {
            byte[] encData_byte = new byte[text.Length];
            encData_byte = Encoding.UTF8.GetBytes(text);
            return  Convert.ToBase64String(encData_byte);
        }


        public static string Decrypt(this string text)
        {
            UTF8Encoding encoder = new System.Text.UTF8Encoding();
            Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(text);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            return new string(decoded_char);
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SoftBBM.Web.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string ConvertToUnSign(this string text)
        {
            text = text.ToLower();
            for (int i = 33; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 58; i < 65; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }

            for (int i = 91; i < 97; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 123; i < 127; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            text = text.Replace("     ", "-");
            text = text.Replace("    ", "-");
            text = text.Replace("   ", "-");
            text = text.Replace("  ", "-");
            text = text.Replace(" ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string RemoveChar(this string text)
        {
            var result = "";
            result = text.Trim().Replace("A", "").Replace("B", "").Replace("C", "").Replace("D", "").Replace("E", "").Replace("F", "").Replace("G", "").Replace("H", "").Replace("I", "").Replace("K", "").Replace("L", "").Replace("M", "").Replace("N", "").Replace("O", "").Replace("P", "").Replace("Q", "").Replace("R", "").Replace("S", "").Replace("T", "").Replace("V", "").Replace("X", "").Replace("a", "").Replace("b", "").Replace("c", "").Replace("d", "").Replace("e", "").Replace("f", "").Replace("g", "").Replace("h", "").Replace("i", "").Replace("k", "").Replace("l", "").Replace("m", "").Replace("n", "").Replace("o", "").Replace("p", "").Replace("q", "").Replace("r", "").Replace("s", "").Replace("t", "").Replace("v", "").Replace("x", "");
            return result;
        }
    }
}
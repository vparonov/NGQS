using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public static class MailLibraryHelperExtensions
    {
        public static MailAddress ToMailAddress(this String str)
        {
            return new MailAddress(str);
        }
        public static String ToBase64String(this String source)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
        }

        public static String FromBase64String(this String source)
        {
            var byteArray = Convert.FromBase64String(source);
            return System.Text.Encoding.Unicode.GetString(byteArray, 0, byteArray.Length); 
        }
        
        public static Encoding ToEncoding(this String str)
        {
            return Encoding.GetEncoding(str);
        }
    }
}

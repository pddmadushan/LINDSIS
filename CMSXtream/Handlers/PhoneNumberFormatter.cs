using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSXtream.Handlers
{
    public class PhoneNumberFormatter
    {
        public static string Format(string input)
        {
            if (input.Length == 9) return $"({input.Substring(0, 3)})-{input.Substring(3, 3)}-{input.Substring(6)}";
            if (input.Length >= 10) return $"({input.Substring(0, 1)})-{input.Substring(1, 3)}-{input.Substring(4, 3)}-{input.Substring(7)}";
            return input;
        }
        public static string AddLeadingZero(string input)
        {
            if (input.Length == 9 && input.Substring(0)!="0")
            {
                return "0" + input;
            } 
            return input.Replace(" ", ""); ;
        }
    }
}

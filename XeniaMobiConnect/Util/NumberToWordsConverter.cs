using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Util
{
    public class NumberToWordsConverter
    {

        public static String[] units = { "", "One", "Two", "Three", "Four",
            "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
            "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
            "Eighteen", "Nineteen" };

        public static String[] tens = {
            "", 		// 0
            "",		// 1
            "Twenty", 	// 2
            "Thirty", 	// 3
            "Forty", 	// 4
            "Fifty", 	// 5
            "Sixty", 	// 6
            "Seventy",	// 7
            "Eighty", 	// 8
            "Ninety" 	// 9
        };

        public static String ConvertAmount(double amount, String wholeWordTitle, String decimalValueTitle)
        {
            try
            {
                int amount_int = (int)amount;
                int amount_dec = (int)Math.Round((amount - (double)(amount_int)) * 100);
                if (amount_dec == 0)
                {
                    return Convert(amount_int) + " " + wholeWordTitle + " " + " Only.";
                }
                else
                {
                    return Convert(amount_int) + " " + wholeWordTitle + " " + " and " + Convert(amount_dec) + " " + decimalValueTitle + " Only.";
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception  
            }
            return "";
        }

        public static String Convert(int n)
        {
            if (n < 0)
            {
                return "Minus " + Convert(-n);
            }

            if (n < 20)
            {
                return units[n];
            }

            if (n < 100)
            {
                return tens[n / 10] + ((n % 10 != 0) ? " " : "") + units[n % 10];
            }

            if (n < 1000)
            {
                return units[n / 100] + " Hundred" + ((n % 100 != 0) ? " " : "") + Convert(n % 100);
            }

            if (n < 100000)
            {
                return Convert(n / 1000) + " Thousand" + ((n % 10000 != 0) ? " " : "") + Convert(n % 1000);
            }

            if (n < 10000000)
            {
                return Convert(n / 100000) + " Lakh" + ((n % 100000 != 0) ? " " : "") + Convert(n % 100000);
            }

            return Convert(n / 10000000) + " Crore" + ((n % 10000000 != 0) ? " " : "") + Convert(n % 10000000);
        }
    }
}
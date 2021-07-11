using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Util
{
    public class ProductUtil
    {

        public static decimal GetKFCessPerc()
        {
            return 1;
        }

        public static double GetKFCess(double amount, double perc)
        {
            double kfCessAmount = amount * (perc / 100);

            return kfCessAmount;
        }

        public static double GetCessAmount(double amount, double perc)
        {
            double cessAmount = amount * (perc / 100);

            return cessAmount;
        }

        public static double GetBasePrice(double price, double taxPercentage, bool isTaxIncl)
        {
            double basePrice = 0;

            if (isTaxIncl)
            {
                basePrice = price / (1 + (taxPercentage / 100));
            }
            else
            {
                basePrice = price;
            }

            return basePrice;
        }

        public static double GetBasePrice(double price, double taxPerc, bool isTaxIncl, double kfCessPerc, double cessPerc, double addCessRate)
        {
            double basePrice = 0;
            double totalTaxPerc = taxPerc + kfCessPerc + cessPerc;
            if (isTaxIncl)
            {
                price = price - addCessRate;
                basePrice = (price) / (1 + (totalTaxPerc / 100));

            }
            else
            {
                basePrice = price;
            }
            return basePrice;
        }

        public static double GetGrossPrice(double price, double discPerc, double taxPerc, Boolean isTaxIncl, double kfCessPerc, double cessPerc, double addCessRate)
        {
            double basePrice = 0;
            double totalTaxPerc = taxPerc + kfCessPerc + cessPerc;
            if (isTaxIncl)
            {
                price = price - addCessRate;
                basePrice = (price) / (1 + (totalTaxPerc / 100));

            }
            else
            {
                basePrice = price;
            }
            double discountAmount = basePrice * (discPerc / 100);
            basePrice = basePrice - discountAmount;
            return basePrice;
        }

        public static double GetNetPrice(double price, double discPerc, double taxPerc, bool isTaxIncl, double kfCessPerc, double cessPerc, double addCessRate)
        {
            double netPrice = 0;
            double tax = 0;
            double totalTaxPerc = taxPerc + kfCessPerc + cessPerc;
            double discountAmount = price * (discPerc / 100);
            price = price - discountAmount;
            if (isTaxIncl)
            {
                netPrice = price;
            }
            else
            {
                tax = (price) * (totalTaxPerc / 100);
                netPrice = price + tax + addCessRate;
            }

            return netPrice;
        }

        public static double GetTaxAmount(double price, double taxPercentage, bool isTaxIncl)
        {
            double taxAmount = 0;

            if (isTaxIncl)
            {
                taxAmount = price - (price / (1 + (taxPercentage / 100)));
            }
            else
            {
                taxAmount = price * (taxPercentage / 100);
            }

            return taxAmount;
        }
    }
}
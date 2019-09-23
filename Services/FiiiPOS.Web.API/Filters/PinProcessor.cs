using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FiiiPay.Framework;
using System.ComponentModel.DataAnnotations;

namespace FiiiPOS.Web.API.Filters
{
    public class PinProcessor
    {
        public static bool IsValid(string cipherPin)
        {
            string pinText;
            try
            {
                pinText = AES128.Decrypt(cipherPin, AES128.DefaultKey);
            }
            catch (Exception)
            {
                return false;
            }

            var reg = new Regex("^\\d{6}$");

            return reg.IsMatch(pinText) && !ContinuousNumber.Contains(pinText);
        }

        public static bool TryParse(string cipherPin,out string pinText)
        {
            try
            {
                pinText = AES128.Decrypt(cipherPin, AES128.DefaultKey);
            }
            catch (Exception)
            {
                pinText = null;
                return false;
            }

            var reg = new Regex("^\\d{6}$");

            return reg.IsMatch(pinText) && !ContinuousNumber.Contains(pinText);
        }

        private static List<string> ContinuousNumber => new List<string>
        {
            //6位连续数字
            "012345",
            "123456",
            "234567",
            "345678",
            "456789",
            "987654",
            "876543",
            "765432",
            "654321",
            "543210",
            //6位重复数字
            "000000",
            "111111",
            "222222",
            "333333",
            "444444",
            "555555",
            "666666",
            "777777",
            "888888",
            "999999"
        };
    }

    public class CollectionMaxCount : ValidationAttribute
    {
        private int _maxCount;

        public CollectionMaxCount(int count)
        {
            _maxCount = count;
        }

        public override bool IsValid(object value)
        {
            var list = value as System.Collections.IEnumerable;

            var count = 0;
            var tempList = list.GetEnumerator();
            while (tempList.MoveNext())
            {
                count++;
                if(count > _maxCount)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
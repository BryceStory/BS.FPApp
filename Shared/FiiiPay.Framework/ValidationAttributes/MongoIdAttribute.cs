using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FiiiPay.Framework.ValidationAttributes
{
    public class MongoIdAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Regex.IsMatch(value.ToString(), "^[0-9a-fA-F]{24}$");
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Invalid {name} format.";
        }
    }
}

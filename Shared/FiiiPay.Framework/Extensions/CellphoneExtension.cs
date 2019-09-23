namespace FiiiPay.Framework
{
    /// <summary>
    /// Class FiiiPay.Framework.CellphoneExtension
    /// </summary>
    public class CellphoneExtension
    {
        /// <summary>
        /// Gets the masked cellphone.
        /// </summary>
        /// <param name="phoneCode">The phone code.</param>
        /// <param name="cellphone">The cellphone.</param>
        /// <returns></returns>
        public static string GetMaskedCellphone(string phoneCode, string cellphone)
        {
            if (string.IsNullOrEmpty(phoneCode) || string.IsNullOrEmpty(cellphone))
            {
                return null;
            }

            return $"{phoneCode} {cellphone.HidePrimaryString(0, 4, '*')}";
        }
    }
}
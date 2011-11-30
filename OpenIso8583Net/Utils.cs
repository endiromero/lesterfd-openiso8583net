using System;
using System.Text;

namespace OpenIso8583Net
{
    /// <summary>
    /// Utilities class with helper functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the luhn digit for a given PAN
        /// </summary>
        /// <param name="pan">PAN missing the luhn check digit</param>
        /// <returns>Luhn check digit</returns>
        public static string GetLuhn(string pan)
        {
            int sum = 0;

            bool alternate = true;
            for (int i = pan.Length - 1; i >= 0; i--)
            {
                int num = int.Parse(pan[i].ToString());

                if (alternate)
                {
                    num *= 2;
                    if (num > 9)
                        num = num - 9;
                }

                sum += num;
                alternate = !alternate;
            }

            int luhnDigit = 10 - (sum % 10);
            if (luhnDigit == 10)
                luhnDigit = 0;

            return luhnDigit.ToString();
        }

        /// <summary>
        /// Checks that the luhn check digit is valid
        /// </summary>
        /// <param name="pan">PAN to validate</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool IsValidPAN(String pan)
        {
            string luhn = GetLuhn(pan.Substring(0, pan.Length - 1));
            return luhn == pan.Substring(pan.Length - 1);
        }

        /// <summary>
        /// PCI DSS PAN mask. For strings longer than 10 chars masks characters [6..Length-4] 
        /// by character 'x'; otherwise returns the pan parameter unchanged.
        /// </summary>
        /// <param name="pan">a PAN string</param>
        /// <returns>a masked PAN string</returns>
        public static string MaskPan(string pan)
        {
            if (pan == null)
                return null;

            const int frontLength = 6;
            const int endLength = 4;
            const int unmaskedLength = frontLength + endLength;

            var totalLength = pan.Length;

            if (totalLength <= unmaskedLength)
                return pan;

            return
                new StringBuilder()
                    .Append(pan.Substring(0, frontLength)) // front
                    .Append(new string('x', totalLength - unmaskedLength))  // mask
                    .Append(pan.Substring((totalLength - endLength), endLength)) // end
                    .ToString();
        }
    }
}

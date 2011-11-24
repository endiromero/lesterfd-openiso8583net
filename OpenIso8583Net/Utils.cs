using System;

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
    }
}

using System;

namespace OpenIso8583Net.Exceptions
{
    /// <summary>
    ///   Exception class for an incorrectly formatted field
    /// </summary>
    public class FieldFormatException : FormatException
    {
        /// <summary>
        ///   Create a new instance of the FieldFormatException class
        /// </summary>
        /// <param name = "fieldNumber"></param>
        /// <param name = "message"></param>
        public FieldFormatException(int fieldNumber, string message)
            : base("Field Number : " + fieldNumber + Environment.NewLine + message)
        {
            FieldNumber = fieldNumber;
        }

        /// <summary>
        ///   Field number that the exception applies to
        /// </summary>
        public int FieldNumber { get; private set; }
    }
}
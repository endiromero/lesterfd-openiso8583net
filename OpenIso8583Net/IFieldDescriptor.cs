using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net
{
    /// <summary>
    /// IFieldDescriptor - an Iso Field definition
    /// </summary>
    /// <remarks>
    /// If you need concurrency keep in mind that a single IFieldDescriptor instance 
    /// is used by multiple IField state keeping instances and thus IFieldDescriptors 
    /// (and all instances they reference) MUST be reentrant, ie. stateless. State 
    /// (context) should be kept inside IFields and passed into IFieldDescriptors
    /// </remarks>
    public interface IFieldDescriptor
    {
        /// <summary>
        ///   The length formatter describing the field
        /// </summary>
        ILengthFormatter LengthFormatter { get; }

        /// <summary>
        ///   The validator describing the field
        /// </summary>
        IFieldValidator Validator { get; }

        /// <summary>
        ///   The field formatter describing the field
        /// </summary>
        IFormatter Formatter { get; }

        /// <summary>
        /// The field adjustment applied during get value and set value
        /// </summary>
        Adjuster Adjuster { get; }

        /// <summary>
        ///   Get the packed length of the field, including a length header if necessary for the given value
        /// </summary>
        /// <param name = "value">Value to calculate length for</param>
        /// <returns>Packed length of the field, including length header</returns>
        int GetPackedLength(string value);

        /// <summary>
        ///   Display the field, used in traces
        /// </summary>
        /// <param name = "prefix">Prefix to display before the field</param>
        /// <param name = "fieldNumber">Field number</param>
        /// <param name = "value">field contents</param>
        /// <returns>formatted string representing the field</returns>
        string Display(string prefix, int fieldNumber, string value);

        /// <summary>
        ///   Unpack the field from the message
        /// </summary>
        /// <param name = "fieldNumber">field number</param>
        /// <param name = "data">message data to unpack from</param>
        /// <param name = "offset">offset in the message to start</param>
        /// <param name = "newOffset">offset at the end of the field for the next field</param>
        /// <returns>valud of the field</returns>
        string Unpack(int fieldNumber, byte[] data, int offset, out int newOffset);

        /// <summary>
        ///   Packs the field into a byte[]
        /// </summary>
        /// <param name = "fieldNumber">number of the field</param>
        /// <param name = "value">Value of the field to pack</param>
        /// <returns>field data packed into a byte[]</returns>
        byte[] Pack(int fieldNumber, string value);
    }
}
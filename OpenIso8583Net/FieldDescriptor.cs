using System;
using System.Text;
using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;

namespace OpenIso8583Net
{
    /// <summary>
    ///   A class describing a field
    /// </summary>
    public class FieldDescriptor : IFieldDescriptor
    {
        /// <summary>
        ///   Describe a field
        /// </summary>
        /// <param name = "lengthFormatter">Length Formatter</param>
        /// <param name = "validator">Validator</param>
        /// <param name = "formatter">Field Formatter</param>
        /// <param name="adjuster">optional Adjuster</param>
        public FieldDescriptor(ILengthFormatter lengthFormatter, IFieldValidator validator,
                               IFormatter formatter, Adjuster adjuster = null)
        {
            if (formatter is BinaryFormatter && !(validator is HexFieldValidator))
                throw new FieldDescriptorException("A Binary field must have a hex validator");

            if (formatter is BcdFormatter && !(validator is NumericFieldValidator))
                throw new FieldDescriptorException("A BCD field must have a numeric validator");

            LengthFormatter = lengthFormatter;
            Validator = validator;
            Formatter = formatter;
            Adjuster = adjuster;
        }

        /// <summary>
        ///   Describe an ASCII field
        /// </summary>
        /// <param name = "lengthFormatter">Length Formatter</param>
        /// <param name = "validator">Validator</param>
        /// <param name="adjuster">optional Adjuster</param>
        public FieldDescriptor(ILengthFormatter lengthFormatter, IFieldValidator validator, Adjuster adjuster = null)
            : this(lengthFormatter, validator, new AsciiFormatter(), adjuster)
        {
        }

        /// <summary>
        ///   The length formatter describing the field
        /// </summary>
        public virtual ILengthFormatter LengthFormatter { get; private set; }

        /// <summary>
        ///   The validator describing the field
        /// </summary>
        public virtual IFieldValidator Validator { get; private set; }

        /// <summary>
        ///   The field formatter describing the field
        /// </summary>
        public virtual IFormatter Formatter { get; private set; }

        /// <summary>
        /// Optional Field Adjuster (may be null)
        /// </summary>
        public Adjuster Adjuster { get; private set; }

        /// <summary>
        ///   Get the packed length of the field, including a length header if necessary for the given value
        /// </summary>
        /// <param name = "value">Value to calculate length for</param>
        /// <returns>Packed length of the field, including length header</returns>
        public virtual int GetPackedLength(string value)
        {
            return LengthFormatter.LengthOfLengthIndicator + Formatter.GetPackedLength(value.Length);
        }

        /// <summary>
        ///   Display the field, used in traces
        /// </summary>
        /// <param name = "prefix">Prefix to display before the field</param>
        /// <param name = "fieldNumber">Field number</param>
        /// <param name = "value">field contents</param>
        /// <returns>formatted string representing the field</returns>
        public virtual string Display(string prefix, int fieldNumber, string value)
        {
            // always use StringBuffer's Append(string) to concatenate user data
            // Formating methods like string.Format("{0}", data) may print garbage when the data contains format characters
            var fieldValue = value == null ? string.Empty : new StringBuilder().Append("[").Append(value).Append("]").ToString();

            return 
                new StringBuilder()
                    .AppendFormat("{0}[{1,-8} {2,-4} {3,6} {4:d4}] {5:d3} {6}",
                                  prefix,
                                  LengthFormatter.Description,
                                  Validator.Description,
                                  LengthFormatter.MaxLength,
                                  Formatter.GetPackedLength(value == null ? 0 : value.Length),
                                  fieldNumber,
                                  fieldValue
                    )
                    .ToString();
        }

        /// <summary>
        ///   Unpack the field from the message
        /// </summary>
        /// <param name = "fieldNumber">field number</param>
        /// <param name = "data">message data to unpack from</param>
        /// <param name = "offset">offset in the message to start</param>
        /// <param name = "newOffset">offset at the end of the field for the next field</param>
        /// <returns>valud of the field</returns>
        public virtual string Unpack(int fieldNumber, byte[] data, int offset, out int newOffset)
        {
            var lenOfLenInd = LengthFormatter.LengthOfLengthIndicator;
            var lengthOfField = LengthFormatter.GetLengthOfField(data, offset);
            if (Formatter is BcdFormatter)
                lengthOfField = Formatter.GetPackedLength(lengthOfField);
            var fieldData = new byte[lengthOfField];
            Array.Copy(data, offset + lenOfLenInd, fieldData, 0, lengthOfField);
            newOffset = offset + lengthOfField + lenOfLenInd;
            var value = Formatter.GetString(fieldData);
            if (!Validator.IsValid(value))
                throw new FieldFormatException(fieldNumber, "Invalid field format");
            var length = value.Length;
            if (Formatter is BinaryFormatter)
                length = Formatter.GetPackedLength(length);
            if (!LengthFormatter.IsValidLength(length))
                throw new FieldLengthException(fieldNumber, "Field is too long");

            return value;
        }

        /// <summary>
        ///   Packs the field into a byte[]
        /// </summary>
        /// <param name = "fieldNumber">number of the field</param>
        /// <param name = "value">Value of the field to pack</param>
        /// <returns>field data packed into a byte[]</returns>
        public virtual byte[] Pack(int fieldNumber, string value)
        {
            if (!LengthFormatter.IsValidLength(Formatter.GetPackedLength(value.Length)))
                throw new FieldLengthException(fieldNumber, "The field length is not valid");
            if (!Validator.IsValid(value))
                throw new FieldFormatException(fieldNumber, "Invalid value for field");

            var lenOfLenInd = LengthFormatter.LengthOfLengthIndicator;
            var lengthOfField = Formatter.GetPackedLength(value.Length);
            var field = new byte[lenOfLenInd + lengthOfField];
            LengthFormatter.Pack(field, value.Length, 0);
            var fieldData = Formatter.GetBytes(value);
            Array.Copy(fieldData, 0, field, lenOfLenInd, lengthOfField);
            return field;
        }

        ///<summary>
        ///  Create and ASCII fixed length field descriptor
        ///</summary>
        ///<param name = "packedLength">The packed length of the field.  For BCD fields, this is half the size of the field you want</param>
        ///<param name = "validator">Validator to use on the field</param>
        ///<returns>field descriptor</returns>
        public static IFieldDescriptor AsciiFixed(int packedLength, IFieldValidator validator)
        {
            return new FieldDescriptor(new FixedLengthFormatter(packedLength), validator, Formatters.Ascii);
        }

        ///<summary>
        ///  Create an ASCII variable length field descriptor
        ///</summary>
        ///<param name = "lengthIndicator">length indicator</param>
        ///<param name = "maxLength">maximum length of the field</param>
        ///<param name = "validator">Validator to use on the field</param>
        ///<returns>field descriptor</returns>
        public static IFieldDescriptor AsciiVar(int lengthIndicator, int maxLength, IFieldValidator validator)
        {
            return new FieldDescriptor(new VariableLengthFormatter(lengthIndicator, maxLength), validator);
        }

        /// <summary>
        /// Create a binary fixed length field
        /// </summary>
        /// <param name = "packedLength">length of the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor BinaryFixed(int packedLength)
        {
            return new FieldDescriptor(new FixedLengthFormatter(packedLength), FieldValidators.Hex, Formatters.Binary);
        }

        /// <summary>
        /// Decorates IFieldDescriptor.Display method with a PCI-DSS PAN mask.
        /// </summary>
        public static IFieldDescriptor PanMask(IFieldDescriptor decoratedFieldDescriptor)
        {
            return new PanMaskDecorator(decoratedFieldDescriptor);
        }

        /// <summary>
        /// Create ASCII packed fixed length auto-padding numeric field (N)
        /// </summary>
        /// <param name = "length">length of the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiNumeric(int length)
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadLeft(length, '0'));
            return new FieldDescriptor(new FixedLengthFormatter(length), FieldValidators.N, setAdjuster);
        }

        /// <summary>
        /// Create ASCII packed fixed length auto-padding alphanumeric field (AN)
        /// </summary>
        /// <param name = "length">length of the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiAlphaNumeric(int length)
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadRight(length, ' '));
            return new FieldDescriptor(new FixedLengthFormatter(length), FieldValidators.Ansp, setAdjuster);
        }

        /// <summary>
        /// Create ASCII packed variable length LL numeric field (LL NUM)
        /// </summary>
        /// <param name="maxLength">maximum field length</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiLlNumeric(int maxLength)
        {
            return AsciiVar(2, maxLength, FieldValidators.N);
        }

        /// <summary>
        /// Create ASCII packed variable length LLL numeric field (LLL NUM)
        /// </summary>
        /// <param name="maxLength">maximum field length</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiLllNumeric(int maxLength)
        {
            return AsciiVar(3, maxLength, FieldValidators.N);
        }

        /// <summary>
        /// Create ASCII packed variable length LL alphanumeric field (LL CHAR)
        /// </summary>
        /// <param name="maxLength">maximum field length</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiLlCharacter(int maxLength)
        {
            return AsciiVar(2, maxLength, FieldValidators.Ans);
        }

        /// <summary>
        /// Create ASCII packed variable length LLL alphanumeric field (LLL CHAR)
        /// </summary>
        /// <param name="maxLength">maximum field length</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiLllCharacter(int maxLength)
        {
            return AsciiVar(3, maxLength, FieldValidators.Ans);
        }

        /// <summary>
        /// Create ASCII packed fixed length auto-padding amount field (AMOUNT)
        /// </summary>
        /// <param name = "length">length of the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiAmount(int length)
        {
            var setAdjuster = new LambdaAdjuster(setLambda: value => value.PadLeft(length, '0'));
            return new FieldDescriptor(new FixedLengthFormatter(length), FieldValidators.Rev87AmountValidator, setAdjuster);
        }

        /// <summary>
        /// Create ASCII packed variable length binary LLL field (LLL BINARY)
        /// </summary>
        /// <param name = "packedLength">length of the field</param>
        /// <returns>field descriptor</returns>
        public static IFieldDescriptor AsciiLllBinary(int packedLength)
        {
            return new FieldDescriptor(new VariableLengthFormatter(3, packedLength), FieldValidators.Hex, Formatters.Binary);
        }
    }
}

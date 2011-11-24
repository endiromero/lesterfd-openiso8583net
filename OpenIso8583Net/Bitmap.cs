using System;
using System.Text;
using OpenIso8583Net.Formatter;

namespace OpenIso8583Net
{
    /// <summary>
    ///   Class representing a bitmap in an ISO 8583 message
    /// </summary>
    public class Bitmap
    {
        private readonly IFormatter _formatter;
        private readonly bool[] _bits;

        /// <summary>
        ///   Create a new instance of the Bitmap class
        /// </summary>
        public Bitmap()
            : this(new BinaryFormatter())
        {
        }

        /// <summary>
        /// Create a new instance of the Bitmap class
        /// </summary>
        /// <param name="formatter">The formatter to use</param>
        public Bitmap(IFormatter formatter)
        {
            _formatter = formatter;
            _bits = new bool[128];
        }

        /// <summary>
        ///   Gets or sets the presence of a field in the bitmap
        /// </summary>
        /// <param name = "field">Field in question</param>
        /// <returns>true if set, false otherwise</returns>
        public bool this[int field]
        {
            get { return IsFieldSet(field); }
            set { SetField(field, value); }
        }

        /// <summary>
        ///   Gets whether or not the extended bitmap is set
        /// </summary>
        public bool IsExtendedBitmap
        {
            get { return IsFieldSet(1); }
        }


        /// <summary>
        ///   Gets the packed length of the message
        /// </summary>
        public int PackedLength
        {
            get { return _formatter.GetPackedLength(IsExtendedBitmap ? 32 : 16); }
        }

        /// <summary>
        ///   Gets if a field is set
        /// </summary>
        /// <param name = "field">Field to query</param>
        /// <returns>true if set, false otherwise</returns>
        public bool IsFieldSet(int field)
        {
            return _bits[field - 1];
        }

        /// <summary>
        ///   Sets a field
        /// </summary>
        /// <param name = "field">Field to set</param>
        /// <param name = "on">Whether or not the field is on</param>
        public void SetField(int field, bool on)
        {
            _bits[field - 1] = on;
            _bits[0] = false;
            for (var i = 64; i <= 127; i++)
                if (_bits[i])
                {
                    _bits[0] = true;
                    break;
                }
        }

        /// <summary>
        ///   Gets the message as a byte array ready to send over the network
        /// </summary>
        /// <returns>byte[] representing the message</returns>
        public virtual byte[] ToMsg()
        {
            var lengthOfBitmap = IsExtendedBitmap ? 16 : 8;
            var data = new byte[lengthOfBitmap];

            for (var i = 0; i < lengthOfBitmap; i++)
                for (var j = 0; j < 8; j++)
                    if (_bits[i * 8 + (j)])
                        data[i] = (byte)(data[i] | (128 / (int)Math.Pow(2, j)));

            if (_formatter is BinaryFormatter)
                return data;

            IFormatter binaryFormatter = new BinaryFormatter();
            var bitmapString = binaryFormatter.GetString(data);

            return _formatter.GetBytes(bitmapString);
        }

        /// <summary>
        ///   Unpacks the bitmap from the message
        /// </summary>
        /// <param name = "msg">byte[] of the full message</param>
        /// <param name = "offset">offset indicating the start of the bitmap</param>
        /// <returns>new offset to start unpacking the first field</returns>
        public int Unpack(byte[] msg, int offset)
        {
            // This is a horribly nasty way of doing the bitmaps, but it works
            // I think...
            var lengthOfBitmap = _formatter.GetPackedLength(16);
            if (_formatter is BinaryFormatter)
            {
                if (msg[offset] >= 128)
                    lengthOfBitmap += 8;
            }
            else
            {
                if (msg[offset] >= 0x38)
                    lengthOfBitmap += 16;
            }

            var bitmapData = new byte[lengthOfBitmap];
            Array.Copy(msg, offset, bitmapData, 0, lengthOfBitmap);

            if (!(_formatter is BinaryFormatter))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                var value = _formatter.GetString(bitmapData);
                bitmapData = binaryFormatter.GetBytes(value);
            }

            // good luck understanding this
            for (var i = 0; i < bitmapData.Length; i++)
                for (var j = 0; j < 8; j++)
                    _bits[i * 8 + j] = (bitmapData[i] & (128 / (int)Math.Pow(2, j))) > 0;

            return offset + lengthOfBitmap;
        }
    }
}
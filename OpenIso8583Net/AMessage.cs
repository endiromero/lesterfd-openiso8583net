﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenIso8583Net
{
    /// <summary>
    ///   Class representing a generic ISO 8583 message
    /// </summary>
    /// <remarks>
    ///   This class has been designed to be overridden and apply to all sorts of Bitmap messages.  As such it
    ///   does not create any fields itself (rev 87 and rev 93) nor does it have an MTID in it so you can use 
    ///   it if you need a sub message as a field.  See <see cref = "Iso8583" /> for an 
    ///   implementation of it
    /// </remarks>
    public abstract class AMessage : IMessage
    {
        /// <summary>
        ///   Bitmap for the ISO message
        /// </summary>
        protected Bitmap _bitmap;

        /// <summary>
        ///   Dictionary containing all the fields in the message
        /// </summary>
        protected Dictionary<int, IField> _fields;

        /// <summary>
        ///   Template describing the ISO message
        /// </summary>
        protected Template _template;

        /// <summary>
        ///   Create a new instance of the message
        /// </summary>
        protected AMessage()
        {
            _template = new Template();
            _fields = new Dictionary<int, IField>();
            _bitmap = new Bitmap();
        }

        /// <summary>
        ///   Gets or sets the value of a field
        /// </summary>
        /// <param name = "field">Field number to get or set</param>
        /// <returns>Value of the field or null if not present</returns>
        public string this[int field]
        {
            get { return GetFieldValue(field); }
            set { SetFieldValue(field, value); }
        }

        /// <summary>
        ///   Gets the packed length of the message
        /// </summary>
        public int PackedLength
        {
            get
            {
                var length = _bitmap.PackedLength;
                for (var i = 2; i <= 128; i++)
                    if (_bitmap[i])
                        length += _fields[i].PackedLength;
                return length;
            }
        }

        /// <summary>
        ///   Gets the processing code (field 3) of the message
        /// </summary>
        public ProcessingCode ProcessingCode
        {
            get { return new ProcessingCode(this[3]); }
        }

        /// <summary>
        /// Describe the packing of the message
        /// </summary>
        /// <returns>the packing of the message</returns>
        public virtual string DescribePacking()
        {
            return _template.DescribePacking();

        }

        #region IMessage Members
        /// <summary>
        ///   Gets the message as a byte array ready to send over the network
        /// </summary>
        /// <returns>byte[] representing the message</returns>
        public virtual byte[] ToMsg()
        {
            var packedLen = PackedLength;
            var data = new byte[packedLen];

            var offset = 0;
            // bitmap
            var bmap = _bitmap.ToMsg();
            Array.Copy(bmap, 0, data, offset, _bitmap.PackedLength);
            offset += _bitmap.PackedLength;
            // Fields
            for (var i = 2; i <= 128; i++)
                if (_bitmap[i])
                {
                    var field = _fields[i];
                    Array.Copy(field.ToMsg(), 0, data, offset, field.PackedLength);
                    offset += field.PackedLength;
                }

            return data;
        }

        /// <summary>
        ///   Unpacks the message from a byte array
        /// </summary>
        /// <param name = "msg">message data to unpack</param>
        /// <param name = "startingOffset">the offset in the array to start</param>
        /// <returns>the offset in the array representing the start of the next message</returns>
        public virtual int Unpack(byte[] msg, int startingOffset)
        {
            var offset = startingOffset;

            // get bitmap
            offset = _bitmap.Unpack(msg, offset);

            // get fields
            for (var i = 2; i <= 128; i++)
                if (_bitmap[i])
                    offset = GetField(i).Unpack(msg, offset);

            return offset;
        }

        #endregion

        /// <summary>
        ///   Clears the field in the message
        /// </summary>
        /// <param name = "field">Field number to clear</param>
        public void ClearField(int field)
        {
            _bitmap[field] = false;
            _fields.Remove(field);
        }

        /// <summary>
        ///   Create a field of the correct type and length
        /// </summary>
        /// <param name = "field">Field number to create</param>
        /// <returns>IField representing the desired field</returns>
        protected abstract IField CreateField(int field);

        /// <summary>
        ///   Get a field from the ISO message
        /// </summary>
        /// <param name = "field">Field to retrieve</param>
        /// <returns>Value of the field or null if not present</returns>
        protected string GetFieldValue(int field)
        {
            return _bitmap[field] ? _fields[field].Value : null;
        }

        /// <summary>
        ///   Gets a field to work on.  Creates the field if it does not exist
        /// </summary>
        /// <param name = "field">Field number to get</param>
        /// <returns>Field to work on</returns>
        protected IField GetField(int field)
        {
            if (!_bitmap[field] || !_fields.ContainsKey(field))
            {
                var f = CreateField(field);
                _fields.Add(field, f);
                _bitmap[field] = true;
            }
            return _fields[field];
        }

        /// <summary>
        ///   Returns if a field is set
        /// </summary>
        /// <param name = "field">Field number to retrieve</param>
        /// <returns>true if set, false otherwise</returns>
        public bool IsFieldSet(int field)
        {
            return _bitmap[field];
        }

        /// <summary>
        ///   Sets a field with the given value in the ISO message.
        /// </summary>
        /// <remarks>
        ///   Don't worry about creating the IField as this method will do so
        /// </remarks>
        /// <param name = "field"></param>
        /// <param name = "value"></param>
        protected void SetFieldValue(int field, string value)
        {
            if (value == null)
            {
                ClearField(field);
                return;
            }
            if (_bitmap[field])
                _fields[field].Value = value;
            else
            {
                var f = CreateField(field);
                f.Value = value;
                _fields.Add(field, f);
                _bitmap[field] = true;
            }
        }

        /// <summary>
        ///   Returns the contents of the message as a string
        /// </summary>
        /// <returns>Pretty printed string</returns>
        public override string ToString()
        {
            return ToString("");
        }

        /// <summary>
        ///   Returns the contents of the message as a string
        /// </summary>
        /// <param name = "prefix">The prefix to apply to each line in the message</param>
        /// <returns>Pretty printed string</returns>
        public virtual string ToString(string prefix)
        {
            var sb = new StringBuilder();

            for (var i = 2; i <= 128; i++)
                if (_bitmap[i])
                    sb.Append(ToString(i, prefix) + Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        ///   Returns the field contents as a string for displaying in traces and the like
        /// </summary>
        /// <param name = "field">Field number</param>
        /// <param name = "prefix">What each line must prepended with the prefix</param>
        /// <returns>Value of the field nicely formatted</returns>
        public virtual string ToString(int field, string prefix)
        {
            return _fields[field].ToString(prefix + "   ");
        }

        #region Nested type: AccountType

        /// <summary>
        ///   Account Types
        /// </summary>
        public static class AccountType
        {
            /// <summary>
            ///   Default
            /// </summary>
            public const string _00_DEFAULT = "00";

            /// <summary>
            ///   Savings
            /// </summary>
            public const string _10_SAVINGS = "10";

            /// <summary>
            ///   Cheque/Check
            /// </summary>
            public const string _20_CHECK = "20";

            /// <summary>
            ///   Credit
            /// </summary>
            public const string _30_CREDIT = "30";

            /// <summary>
            ///   Universal
            /// </summary>
            public const string _40_UNIVERSAL = "40";

            /// <summary>
            ///   Investment
            /// </summary>
            public const string _50_INVESTMENT = "50";

            /// <summary>
            ///   Electronic purse default
            /// </summary>
            public const string _60_ELECTRONIC_PURSE_DEFAULT = "60";
        }

        #endregion

        #region Nested type: AmountType

        /// <summary>
        ///   Amount Types
        /// </summary>
        public static class AmountType
        {
            /// <summary>
            ///   Ledger Balance
            /// </summary>
            public const string _01_LEDGER_BALANCE = "01";

            /// <summary>
            ///   Available Balance
            /// </summary>
            public const string _02_AVAILABLE_BALANCE = "02";

            /// <summary>
            ///   Owing
            /// </summary>
            public const string _03_OWING = "03";

            /// <summary>
            ///   Due
            /// </summary>
            public const string _04_DUE = "04";

            /// <summary>
            ///   Remaining this cycle
            /// </summary>
            public const string _20_REMAINING_THIS_CYCLE = "20";

            /// <summary>
            ///   Cash
            /// </summary>
            public const string _40_CASH = "40";

            /// <summary>
            ///   Goods and Services
            /// </summary>
            public const string _41_GOODS_SERVICES = "41";

            /// <summary>
            ///   Approved
            /// </summary>
            public const string _53_APPROVED = "53";

            /// <summary>
            ///   Tip
            /// </summary>
            public const string _56_TIP = "56";

            /// <summary>
            ///   Available Credit
            /// </summary>
            public const string _90_AVAILABLE_CREDIT = "90";

            /// <summary>
            ///   Credit Limit
            /// </summary>
            public const string _91_CREDIT_LIMIT = "91";
        }

        #endregion
    }
}
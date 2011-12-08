using System.Collections.Generic;
using System.Text;
using OpenIso8583Net.Formatter;

namespace OpenIso8583Net
{
    /// <summary>
    ///   A Template describing a message
    /// </summary>
    public class Template : Dictionary<int, IFieldDescriptor>
    {
        /// <summary>
        ///   Create a new instance of the Template class
        /// </summary>
        public Template()
        {
            MsgTypeFormatter = Formatters.Ascii;
        }

        /// <summary>
        ///   Message type formatter
        /// </summary>
        public IFormatter MsgTypeFormatter { get; set; }

        /// <summary>
        ///   Describe the packing format of the template
        /// </summary>
        /// <returns>The packing of the template</returns>
        public string DescribePacking()
        {
            var sb = new StringBuilder();

            foreach (var kvp in this)
            {
                var field = kvp.Key;
                var descriptor = kvp.Value;
                sb.AppendLine(descriptor.Display(string.Empty, field, null));
            }

            return sb.ToString();
        }
    }
}
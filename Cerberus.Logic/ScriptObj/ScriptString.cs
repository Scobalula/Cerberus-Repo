using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a Script String
    /// </summary>
    public class ScriptString
    {
        /// <summary>
        /// Internal Value
        /// </summary>
        private string m_Value { get; set; }

        /// <summary>
        /// Gets or Sets the String Value, on set the string is santised of any escape characters
        /// </summary>
        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value.
                    Replace("\\", "\\\\").
                    Replace("\"", "\\\"").
                    Replace("\b", "\\b").
                    Replace("\f", "\\f").
                    Replace("\n", "\\n").
                    Replace("\r", "\\r").
                    Replace("\t", "\\t");
            }
        }

        /// <summary>
        /// Gets or Sets the offset to the String Value
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or Sets the list of references within the byte code
        /// </summary>
        public List<int> References { get; set; }
    }
}

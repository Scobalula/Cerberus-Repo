using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus
{
    /// <summary>
    /// A class to hold a Script Include
    /// </summary>
    public class ScriptInclude : IComparable
    {
        /// <summary>
        /// Internal Value
        /// </summary>
        private string m_Value { get; set; }

        /// <summary>
        /// Gets or Sets the Include Path Value, on set the path is santised
        /// </summary>
        public string Path
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value.Replace("/", "\\");
            }
        }

        /// <summary>
        /// Creates a new Script Include
        /// </summary>
        public ScriptInclude(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Returns the Path of the Include
        /// </summary>
        public override string ToString()
        {
            return Path;
        }

        /// <summary>
        /// Compares the paths of includes
        /// </summary>
        public int CompareTo(object obj)
        {
            if(obj is ScriptInclude include)
            {
                return Path.CompareTo(include.Path);
            }

            return -1;
        }
    }
}

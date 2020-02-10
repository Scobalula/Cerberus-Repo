using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a GSC/CSC Function Import
    /// </summary>
    public class ScriptImport
    {
        /// <summary>
        /// Gets or Sets the Name of this Import
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the Namespace for this Import
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or Sets the number of Parameters
        /// </summary>
        public int ParameterCount { get; set; }

        /// <summary>
        /// Gets or Sets the list of references within the byte code
        /// </summary>
        public List<int> References { get; set; }
    }
}

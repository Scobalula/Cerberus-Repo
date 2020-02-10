using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    public class ScriptOp
    {
        /// <summary>
        /// Gets or Sets the Metatadata
        /// </summary>
        public ScriptOpMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or Sets the offset to this operation
        /// </summary>
        public int OpCodeOffset { get; set; }

        /// <summary>
        /// Gets or Sets the size of this operation
        /// </summary>
        public int OpCodeSize { get; set; }

        /// <summary>
        /// Whether or not this operation was visited by the decompiler
        /// </summary>
        public bool Visited = false;

        /// <summary>
        /// List of Operation Operands
        /// </summary>
        public List<ScriptOpOperand> Operands = new List<ScriptOpOperand>();
    }
}

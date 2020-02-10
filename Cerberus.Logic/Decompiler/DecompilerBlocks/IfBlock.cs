using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold an if Block
    /// </summary>
    internal class IfBlock : DecompilerBlock
    {
        /// <summary>
        /// The comparison being made
        /// </summary>
        public string Comparison { get; set; }

        /// <summary>
        /// Initializes an if Block
        /// </summary>
        public IfBlock(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        /// <summary>
        /// Gets the header
        /// </summary>
        public override string GetHeader() => string.Format("if({0})", Comparison);

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a switch statement
    /// </summary>
    internal class SwitchBlock : DecompilerBlock
    {
        /// <summary>
        /// The value we're switching on
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a Switch Block
        /// </summary>
        public SwitchBlock(int startOffset, int endOffset) : base(startOffset, endOffset) { }


        public override string GetHeader()
        {
            return string.Format("switch({0})", Value);
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

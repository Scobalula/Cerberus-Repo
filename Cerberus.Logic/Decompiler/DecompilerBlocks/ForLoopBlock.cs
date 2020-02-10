using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a For Loop
    /// </summary>
    internal class ForLoopBlock : DecompilerBlock
    {
        /// <summary>
        /// The initializer (i.e. i = 0)
        /// </summary>
        public string Initializer { get; set; }

        /// <summary>
        /// The comparison being made
        /// </summary>
        public string Comparison { get; set; }

        /// <summary>
        /// The modifier (i.e. i++)
        /// </summary>
        public string Modifier { get; set; }


        public ForLoopBlock(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        public override string GetHeader()
        {
            return string.Format("for({0}; {1}; {2})", Initializer, Comparison, Modifier);
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

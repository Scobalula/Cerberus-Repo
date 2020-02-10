using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a ForEver loop (for(;;))
    /// </summary>
    internal class ForEver : DecompilerBlock
    {
        public ForEver(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        public override string GetHeader()
        {
            return "for(;;)";
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    internal class ElseBlock : DecompilerBlock
    {
        public ElseBlock(int startOffset, int endOffset) : base(startOffset, endOffset)
        {
        }

        public override string GetHeader()
        {
            return "else";
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

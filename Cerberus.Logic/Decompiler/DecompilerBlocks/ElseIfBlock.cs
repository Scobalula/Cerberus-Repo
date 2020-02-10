using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    internal class ElseIfBlock : DecompilerBlock
    {
        public string Comparison { get; set; }

        public ElseIfBlock(int startOffset, int endOffset) : base(startOffset, endOffset)
        {
        }

        public override string GetHeader()
        {
            return string.Format("else if({0})", Comparison);
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

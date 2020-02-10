using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a While Loop
    /// </summary>
    internal class DoWhileLoop : DecompilerBlock
    {
        public string Comparison { get; set; }
        public DoWhileLoop(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        public override string GetHeader()
        {
            return "do";
        }

        public override string GetFooter()
        {
            return string.Format("while({0});", Comparison);
        }
    }
}

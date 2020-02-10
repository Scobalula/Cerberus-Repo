using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a basic block
    /// 
    /// This class is primarily used during the initial phase of the decompiler
    /// And for the root block
    /// </summary>
    internal class BasicBlock : DecompilerBlock
    {
        public BasicBlock(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        public override string GetFooter()
        {
            return null;
        }

        public override string GetHeader()
        {
            return null;
        }
    }
}

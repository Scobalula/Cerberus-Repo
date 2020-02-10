using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.IO;

namespace Cerberus.Logic
{
    internal abstract class DecompilerBlock
    {
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public int CodeStartOffset { get; set; }
        public int CodeEndOffset { get; set; }
        public int ContinueOffset = -1;
        public int BreakOffset = -1;
        public bool RequiresBraces = true;
        public bool CanSkipZeroSize = false;
        public bool Visited = false;
        public List<string> Lines = new List<string>();

        public List<int> ChildBlockIndices = new List<int>();

        public DecompilerBlock(int startOffset, int endOffset)
        {
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public abstract string GetHeader();
        public abstract string GetFooter();
    }
}

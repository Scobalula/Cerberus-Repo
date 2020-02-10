using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    public class ScriptOpSwitch
    {
        public string CaseValue;
        public int ByteCodeOffset;
        public int OriginalIndex;

        public override string ToString()
        {
            return string.Format("Case({0}, {1})", CaseValue, ByteCodeOffset);
        }
    }
}

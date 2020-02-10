using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a GSC/CSC Export/Function
    /// </summary>
    public class ScriptExport
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public uint Checksum { get; set; }
        public int ByteCodeOffset { get; set; }
        public int ByteCodeSize { get; set; }
        public int ParameterCount { get; set; }
        public int DisassemblyLine { get; set; }
        public int DecompilerLine { get; set; }
        public ScriptExportFlags Flags { get; set; }
        public List<ScriptOp> Operations = new List<ScriptOp>();
    }
}

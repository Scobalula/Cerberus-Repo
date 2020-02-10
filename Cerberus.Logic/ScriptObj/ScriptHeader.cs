using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a GSC/CSC Header
    /// </summary>
    public class ScriptHeader
    {
        /// <summary>
        /// Gets or Sets the Offset to the Name of this Script
        /// </summary>
        public int NameOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Source File Checksum (CRC32)
        /// </summary>
        public uint SourceChecksum { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Include Table
        /// </summary>
        public int IncludeTableOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Number of Includes in the Include Table
        /// </summary>
        public int IncludeCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Anim Tree Table
        /// </summary>
        public int AnimTreeTableOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Number of Anim Trees in the Anim Tree Table
        /// </summary>
        public int AnimTreeCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Byte Code Section
        /// </summary>
        public int ByteCodeOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Size of the Byte Code Section
        /// </summary>
        public int ByteCodeSize { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the String Table Section
        /// </summary>
        public int StringTableOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Number of Strings in the String Table
        /// </summary>
        public int StringCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Debug String Table Section (scriptgdb)
        /// </summary>
        public int DebugStringTableOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Number of Debug Strings in the Debug String Table
        /// </summary>
        public int DebugStringCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Exports Table Section
        /// </summary>
        public int ExportTableOffset {get; set; }

        /// <summary>
        /// Gets or Sets the Number of Exports in the Exports Table
        /// </summary>
        public int ExportsCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Imports Table Section
        /// </summary>
        public int ImportTableOffset {get; set; }

        /// <summary>
        /// Gets or Sets the Number of Imports in the Imports Table
        /// </summary>
        public int ImportsCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Fixup Table Section
        /// </summary>
        public int FixupTableOffset {get; set; }

        /// <summary>
        /// Gets or Sets the Number of Fixups in the Fixup Table
        /// </summary>
        public int FixupCount { get; set; }

        /// <summary>
        /// Gets or Sets the Offset to the Profile Table Section
        /// </summary>
        public int ProfileTableOffset { get; set; }

        /// <summary>
        /// Gets or Sets the Number of Imports in the Imports Table
        /// </summary>
        public int ProfileCount { get; set; }

        /// <summary>
        /// Gets or Sets the Misc. Flags
        /// </summary>
        public int Flags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cerberus.Logic
{
    /// <summary>
    /// Base Script File Class
    /// </summary>
    public abstract class ScriptBase : IDisposable
    {
        /// <summary>
        /// Gets or Sets the Script File Path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the Script File Name
        /// </summary>
        public string FileName => Path.GetFileName(FilePath);

        /// <summary>
        /// Gets or Sets the name of the Game
        /// </summary>
        public abstract string Game { get; }

        /// <summary>
        /// Gets or Sets the Hash Table of Names, DVARs, etc.
        /// </summary>
        public Dictionary<uint, string> HashTable { get; set; }

        /// <summary>
        /// Gets or Sets the Script Data Stream
        /// </summary>
        public BinaryReader Reader { get; set; }

        /// <summary>
        /// Size of the Data Stream as a KB String
        /// </summary>
        public string DisplaySize => string.Format("{0:0.000} KB", Reader.BaseStream.Position / 1000.0);

        /// <summary>
        /// Gets or Sets the Script Header
        /// </summary>
        public ScriptHeader Header { get; set; }

        /// <summary>
        /// Gets or Sets the list of Script Includes
        /// </summary>
        public List<ScriptInclude> Includes { get; set; }

        /// <summary>
        /// Gets or Sets the list of Script Strings
        /// </summary>
        public List<ScriptString> Strings { get; set; }

        /// <summary>
        /// Gets or Sets the list of Script Imports
        /// </summary>
        public List<ScriptExport> Exports { get; set; }

        /// <summary>
        /// Gets or Sets the list of Script Exports
        /// </summary>
        public List<ScriptImport> Imports { get; set; }

        /// <summary>
        /// Gets or Sets the list of Script Exports
        /// </summary>
        public List<ScriptAnimTree> AnimTrees { get; set; }

        /// <summary>
        /// Hash References that weren't replaced
        /// </summary>
        public Dictionary<uint, string> HashReferences = new Dictionary<uint, string>();

        /// <summary>
        /// Initializes an instance of the Script Class
        /// </summary>
        public ScriptBase(BinaryReader reader, Dictionary<uint, string> hashTable)
        {
            Reader = reader;
            HashTable = hashTable;
            LoadHeader();
            LoadIncludes();
            LoadAnimTrees();
            LoadStrings();
            LoadImports();
            LoadExports();
        }

        /// <summary>
        /// Initializes an instance of the Script Class
        /// </summary>
        public ScriptBase(Stream stream, Dictionary<uint, string> hashTable) : this(new BinaryReader(stream), hashTable) { }

        /// <summary>
        /// Loads the Header from the GSC File
        /// </summary>
        public abstract void LoadHeader();
        public abstract void LoadIncludes();
        public abstract void LoadAnimTrees();
        public abstract void LoadStrings();
        public abstract void LoadImports();
        public abstract void LoadExports();
        public abstract List<ScriptOpSwitch> LoadEndSwitch();
        public abstract int GetJumpLocation(int from, int to);
        public abstract ScriptOp LoadOperation(int offset);

        public void LoadFunction(ScriptExport function)
        {
            var offset = function.ByteCodeOffset;
            var endOffset = function.ByteCodeOffset + function.ByteCodeSize;

            while (offset <= endOffset)
            {
                var operation = LoadOperation(offset);

                if (operation == null)
                {
                    //function.Operations.Add(new ScriptOp()
                    //{
                    //    Metadata = new ScriptOpMetadata(ScriptOpCode.Invalid, ScriptOpType.None, ScriptOperandType.None),
                    //    OpCodeOffset = offset
                    //});
                    break;
                }

                offset += operation.OpCodeSize;
                function.Operations.Add(operation);
            }
        }

        /// <summary>
        /// Disassembles the entire script and returns a string containing the disassembly
        /// </summary>
        public string Disassemble()
        {
            // Keep track of the line number for UI
            var lineNumber = 0;
            var output = new StringBuilder();

            foreach(var include in Includes)
            {
                output.AppendLine(string.Format("#using {0};", include));
                lineNumber++;
            }

            // Add a space
            if (Includes.Count > 0)
            {
                output.AppendLine();
                lineNumber++;
            }

            foreach (var function in Exports)
            {
                try
                {
                    // Spit out some info
                    output.AppendLine("/*");
                    output.AppendLine(string.Format("\tName: {0}", function.Name));
                    output.AppendLine(string.Format("\tNamespace: {0}", function.Namespace));
                    output.AppendLine(string.Format("\tChecksum: 0x{0:X}", function.Checksum));
                    output.AppendLine(string.Format("\tOffset: 0x{0:X}", function.ByteCodeOffset));
                    output.AppendLine(string.Format("\tSize: 0x{0:X}", function.ByteCodeSize));
                    output.AppendLine(string.Format("\tParameters: {0}", function.ParameterCount));
                    output.AppendLine(string.Format("\tFlags: {0}", function.Flags));
                    output.AppendLine("*/");
                    lineNumber += 9;

                    // Use the liner number AFTER the info above, we want to go
                    // to the literal start
                    function.DisassemblyLine = lineNumber;

                    // If we have a namespace we can add it, for decompiler we'll use
                    // #namespace but for disassembly we'll add it to the call
                    output.AppendLine(string.Format("function {0}{1}(...)",
                        string.IsNullOrWhiteSpace(function.Namespace) ? "" : function.Namespace + "::",
                        function.Name));
                    output.AppendLine("{");
                    lineNumber += 2;

                    foreach(var operation in function.Operations)
                    {
                        // Add IP and Size Info
                        output.AppendFormat("\t/* IP: 0x{0} - Size 0x{1} */\t\t\tOP_{2}(",
                            operation.OpCodeOffset.ToString("X8"),
                            operation.OpCodeSize.ToString("X8"),
                            operation.Metadata.OpCode);

                        for (int i = 0; i < operation.Operands.Count; i++)
                        {
                            output.AppendFormat("{0}{1}", operation.Operands[i].Value, i == operation.Operands.Count - 1 ? "" : ", ");
                        }

                        output.AppendLine(");");

                        lineNumber++;
                    }

                    output.AppendLine("}");
                    lineNumber++;
                }
                catch(Exception e)
                {
                    output.AppendLine("/* " + e.ToString() + " */");
                    lineNumber += e.ToString().Split('\n').Length;
                    output.AppendLine("}");
                }
            }

            return output.ToString();
        }

        public string Decompile()
        {
            // Keep track of the line number for UI
            var output = new StringBuilder();
            int lineNumber = 0;

            // We need to store the current namespace
            var nameSpace = "";

            foreach (var include in Includes)
            {
                output.AppendLine(string.Format("#using {0};", include));
                lineNumber++;
            }

            // Add a space
            if (Includes.Count > 0)
            {
                output.AppendLine();
                lineNumber++;
            }

            foreach (var animTree in AnimTrees)
            {
                output.AppendLine(string.Format("#using_animtree(\"{0}\");", animTree.Name));
                lineNumber++;
            }

            // Add a space
            if (AnimTrees.Count > 0)
            {
                output.AppendLine();
                lineNumber++;
            }

            foreach (var function in Exports)
            {
                // Write the namspace if it differs
                if (!string.IsNullOrWhiteSpace(function.Namespace) && function.Namespace != nameSpace)
                {
                    nameSpace = function.Namespace;
                    output.AppendLine(string.Format("#namespace {0};\n", nameSpace));
                    lineNumber += 2;
                }

                // Spit out some info
                output.AppendLine("/*");
                output.AppendLine(string.Format("\tName: {0}", function.Name));
                output.AppendLine(string.Format("\tNamespace: {0}", function.Namespace));
                output.AppendLine(string.Format("\tChecksum: 0x{0:X}", function.Checksum));
                output.AppendLine(string.Format("\tOffset: 0x{0:X}", function.ByteCodeOffset));
                output.AppendLine(string.Format("\tSize: 0x{0:X}", function.ByteCodeSize));
                output.AppendLine(string.Format("\tParameters: {0}", function.ParameterCount));
                output.AppendLine(string.Format("\tFlags: {0}", function.Flags));
                output.AppendLine("*/");
                lineNumber += 9;

                using (var decompiler = new Decompiler(function, this))
                {
                    function.DecompilerLine = lineNumber;
                    var result = decompiler.GetWriterOutput();
                    output.Append(result);
                    output.AppendLine();
                    lineNumber += Utility.GetLineCount(result);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Exports Hash Table (unnamed variables, etc.)
        /// </summary>
        /// <returns></returns>
        public string ExportHashTable()
        {
            var output = new StringBuilder();

            output.AppendLine("hash,name");

            foreach(var hashVal in HashReferences)
            {
                output.AppendFormat("0x{0:X},{1}\n", hashVal.Key, hashVal.Value);
            }

            return output.ToString();
        }

        /// <summary>
        /// Gets the string for the given hash, otherwise returns the default value or a formatted hex value
        /// </summary>
        public string GetHashValue(uint value, string prefix = "__", string defaultVal = "")
        {
            // Check if it's in the hash table
            if(HashTable.TryGetValue(value, out var result))
            {
                return result;
            }

            // If we have a default value (i.e. Bo3 var names) use that
            if (!string.IsNullOrWhiteSpace(defaultVal))
            {
                return defaultVal;
            }

            // Otherwise we're just using the formatted result
            var hashed = string.Format("{0}{1:x}", prefix, value);

            // Add it to the list
            if(!HashReferences.ContainsKey(value))
            {
                HashReferences.Add(value, hashed);
            }

            return hashed;
        }

        /// <summary>
        /// Gets the string by pointer reference
        /// </summary>
        public ScriptString GetString(int ptr)
        {
            return Strings.Where(x => x.References.Contains(ptr)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the import by pointer reference
        /// </summary>
        public ScriptImport GetImport(int ptr)
        {
            return Imports.Where(x => x.References.Contains(ptr)).FirstOrDefault();
        }

        /// <summary>
        /// Disposes of the Reader
        /// </summary>
        public void Dispose()
        {
            Reader.Dispose();
        }

        /// <summary>
        /// Loads the given script using the respective game class
        /// </summary>
        /// <param name="reader">Reader/Stream</param>
        public static ScriptBase LoadScript(BinaryReader reader, Dictionary<string, Dictionary<uint, string>> hashTables)
        {
            // We can use the magic to determine game
            switch(reader.ReadUInt64())
            {
                case 0x1C000A0D43534780:
                    return new BlackOps3Script(reader, hashTables["BlackOps3"]);
                case 0x6000A0D43534780:
                    return new BlackOps2Script(reader, hashTables["BlackOps2"]);
                default:
                    throw new ArgumentException("Invalid Script Magic Number.", "Magic");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PhilLibX.IO;

namespace Cerberus.Logic
{
    /// <summary>
    /// Class to process Black Ops II Scripts
    /// </summary>
    public class BlackOps2Script : ScriptBase
    {
        /// <summary>
        /// Returns the Game Name for Black Ops II
        /// </summary>
        public override string Game => "Black Ops II";

        /// <summary>
        /// Creates an instance of a new Black Ops II Script with a Stream
        /// </summary>
        public BlackOps2Script(Stream stream, Dictionary<uint, string> hashTable) : base(stream, hashTable) { }

        /// <summary>
        /// Creates an instance of a new Black Ops II Script with a Reader
        /// </summary>
        public BlackOps2Script(BinaryReader reader, Dictionary<uint, string> hashTable) : base(reader, hashTable) { }

        /// <summary>
        /// Loads the header from a Black Ops II Script
        /// </summary>
        public override void LoadHeader()
        {
            // Ensure we're at the header (skip 8 byte magic)
            Reader.BaseStream.Position = 8;

            Header = new ScriptHeader()
            {
                SourceChecksum         = Reader.ReadUInt32(),
                IncludeTableOffset     = Reader.ReadInt32(),
                AnimTreeTableOffset    = Reader.ReadInt32(),
                ByteCodeOffset         = Reader.ReadInt32(),
                StringTableOffset      = Reader.ReadInt32(),
                DebugStringTableOffset = 0, // Black Ops 2 GSCs do not contain a Debug String Table
                ExportTableOffset      = Reader.ReadInt32(),
                ImportTableOffset      = Reader.ReadInt32(),
                FixupTableOffset       = Reader.ReadInt32(),
                ProfileTableOffset     = Reader.ReadInt32(),
                ByteCodeSize           = Reader.ReadInt32(),
                NameOffset             = Reader.ReadInt16(),
                StringCount            = Reader.ReadInt16(),
                ExportsCount           = Reader.ReadInt16(),
                ImportsCount           = Reader.ReadInt16(),
                FixupCount             = Reader.ReadInt16(),
                ProfileCount           = Reader.ReadInt16(),
                DebugStringCount       = 0,
                IncludeCount           = Reader.ReadByte(),
                AnimTreeCount          = Reader.ReadByte(),
                Flags                  = Reader.ReadByte()
            };

            // Get name of this script from the header
            Reader.BaseStream.Position = Header.NameOffset;
            FilePath = Reader.ReadNullTerminatedString();

            // Skip padding (header is 64 bytes in total)
            Reader.BaseStream.Position = 64;
        }

        /// <summary>
        /// Loads strings from a Black Ops II Script
        /// </summary>
        public override void LoadStrings()
        {
            Reader.BaseStream.Position = Header.StringTableOffset;

            Strings = new List<ScriptString>(Header.StringCount + Header.DebugStringCount);

            for (int i = 0; i < Header.StringCount; i++)
            {
                var scriptString = new ScriptString()
                {
                    Offset = Reader.ReadUInt16(),
                    References = new List<int>()
                };

                var referenceCount = Reader.ReadByte();
                Reader.BaseStream.Position += 1;

                // We need to store the references as we'll use them
                // for resolving strings instead of using the pointers
                for (int j = 0; j < referenceCount; j++)
                {
                    scriptString.References.Add(Reader.ReadInt32());
                }

                scriptString.Value = Reader.PeekNullTerminatedString(scriptString.Offset);

                Strings.Add(scriptString);
            }
        }

        /// <summary>
        /// Loads exports from a Black Ops II Script
        /// </summary>
        public override void LoadExports()
        {
            Reader.BaseStream.Position = Header.ExportTableOffset;

            Exports = new List<ScriptExport>(Header.ExportsCount);

            for(int i = 0; i < Header.ExportsCount; i++)
            {
                var export = new ScriptExport()
                {
                    Checksum       = Reader.ReadUInt32(),
                    ByteCodeOffset = Reader.ReadInt32(),
                    Name           = Reader.PeekNullTerminatedString(Reader.ReadUInt16()),
                    // Use the file name as namespace like Bo3 does if none is present
                    Namespace      = Path.GetFileNameWithoutExtension(FilePath.Replace("/", "\\")),
                    ParameterCount = Reader.ReadByte(),
                    Flags          = (ScriptExportFlags)Reader.ReadByte()
                };

                var crc32 = new CRC32();

                // Store our current position as we'll need to return back here
                var offset = Reader.BaseStream.Position;
                Reader.BaseStream.Position = export.ByteCodeOffset;
                var byteCodeSize = 0;

                // From kokole/Nukem's, brute force via CRC32
                // This will only work on files dumped from a fast file
                while (true)
                {
                    crc32.Update(Reader.ReadByte());

                    // If we hit, we're done
                    if(crc32.Value == export.Checksum)
                    {
                        break;
                    }

                    byteCodeSize += 1;
                }

                // We can now use this - the start as our size
                export.ByteCodeSize = byteCodeSize;

                LoadFunction(export);

                // Go back to the table
                Reader.BaseStream.Position = offset;

                Exports.Add(export);
            }
        }

        public override void LoadImports()
        {
            Reader.BaseStream.Position = Header.ImportTableOffset;

            Imports = new List<ScriptImport>(Header.ImportsCount);

            for (int i = 0; i < Header.ImportsCount; i++)
            {
                var import = new ScriptImport()
                {
                    Name = Reader.PeekNullTerminatedString(Reader.ReadUInt16()),
                    Namespace = Reader.PeekNullTerminatedString(Reader.ReadUInt16()),
                    References = new List<int>()
                };

                var referenceCount = Reader.ReadUInt16();
                import.ParameterCount = Reader.ReadByte();
                Reader.BaseStream.Position += 1;

                for (int j = 0; j < referenceCount; j++)
                {
                    import.References.Add(Reader.ReadInt32());
                }

                Imports.Add(import);
            }
        }

        public override void LoadIncludes()
        {
            Reader.BaseStream.Position = Header.IncludeTableOffset;

            Includes = new List<ScriptInclude>(Header.IncludeCount);

            for (int i = 0; i < Header.IncludeCount; i++)
            {
                Includes.Add(new ScriptInclude(Reader.PeekNullTerminatedString(Reader.ReadInt32())));
            }
        }

        public override ScriptOp LoadOperation(int offset)
        {
            Reader.BaseStream.Position = offset;
            var opCodeIndex = Reader.ReadByte();

            // 0x7B is the literal highest for Bo2
            if (opCodeIndex > 0x7B)
            {
                return null;
            }

            ScriptOp operation = new ScriptOp()
            {
                Metadata = ScriptOpMetadata.OperationInfo[opCodeIndex],
                OpCodeOffset = (int)Reader.BaseStream.Position - 1,
            };

            // Use a type rather than large switch for each operation
            // so we can easily fix bugs and adjust across multiple op codes
            switch (operation.Metadata.OperandType)
            {
                case ScriptOperandType.None:
                    {
                        break;
                    }
                case ScriptOperandType.Int8:
                    {
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadSByte()));
                        break;
                    }
                case ScriptOperandType.UInt8:
                    {
                        if(operation.Metadata.OpCode == ScriptOpCode.GetNegByte)
                        {
                            operation.Operands.Add(new ScriptOpOperand(-Reader.ReadByte()));
                        }
                        else
                        {
                            operation.Operands.Add(new ScriptOpOperand(Reader.ReadByte()));
                        }
                        break;
                    }
                case ScriptOperandType.Int16:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadInt16()));
                        break;
                    }
                case ScriptOperandType.UInt16:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                        if (operation.Metadata.OpCode == ScriptOpCode.GetNegUnsignedShort)
                        {
                            operation.Operands.Add(new ScriptOpOperand(-Reader.ReadUInt16()));
                        }
                        else
                        {
                            operation.Operands.Add(new ScriptOpOperand(Reader.ReadUInt16()));
                        }
                        break;
                    }
                case ScriptOperandType.Int32:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadInt32()));
                        break;
                    }
                case ScriptOperandType.UInt32:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadUInt32()));
                        break;
                    }
                case ScriptOperandType.Hash:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(GetHashValue(Reader.ReadUInt32(), "hash_")));
                        break;
                    }
                case ScriptOperandType.Float:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadSingle()));
                        break;
                    }
                case ScriptOperandType.Vector:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle()));
                        break;
                    }
                case ScriptOperandType.VectorFlags:
                    {
                        var flags = Reader.ReadByte();

                        // Set each flag, it's either 1.0, -1.0, or simply 0.0
                        operation.Operands.Add(new ScriptOpOperand(
                            (flags & 0x20) != 0 ? 1.0f : (flags & 0x10) != 0 ? -1.0f : 0.0f,
                            (flags & 0x08) != 0 ? 1.0f : (flags & 0x04) != 0 ? -1.0f : 0.0f,
                            (flags & 0x02) != 0 ? 1.0f : (flags & 0x01) != 0 ? -1.0f : 0.0f));
                        break;
                    }
                case ScriptOperandType.VariableName:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                        operation.Operands.Add(new ScriptOpOperand(Reader.PeekNullTerminatedString(Reader.ReadUInt16())));
                        break;
                    }
                case ScriptOperandType.String:
                    {
                        // If it's anim animation, etc. we can just read at the location, but for strings
                        // we can just grab via pointer
                        switch(operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.GetString:
                                Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                                operation.Operands.Add(new ScriptOpOperand("\"" + GetString((int)Reader.BaseStream.Position).Value + "\""));
                                Reader.BaseStream.Position += 2;
                                break;
                            case ScriptOpCode.GetIString:
                                Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                                operation.Operands.Add(new ScriptOpOperand("&\"" + GetString((int)Reader.BaseStream.Position).Value + "\""));
                                Reader.BaseStream.Position += 2;
                                break;
                            default:
                                Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                                operation.Operands.Add(new ScriptOpOperand("%" + Reader.PeekNullTerminatedString(Reader.ReadInt32())));
                                break;
                        }
                        
                        break;
                    }
                case ScriptOperandType.FunctionPointer:
                    {
                        Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                        operation.Operands.Add(new ScriptOpOperand(Reader.PeekNullTerminatedString(Reader.ReadInt32())));
                        break;
                    }
                case ScriptOperandType.Call:
                    {
                        // Skip param count, it isn't stored here until in memory
                        Reader.BaseStream.Position += 1;
                        try
                        {
                            Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
                            operation.Operands.Add(new ScriptOpOperand(Reader.PeekNullTerminatedString(Reader.ReadInt32())));
                        }
                        catch
                        {
                            return null;
                        }
                        break;
                    }
                case ScriptOperandType.VariableList:
                    {
                        var varCount = Reader.ReadByte();

                        for(int i = 0; i < varCount; i++)
                        {
                            Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 2);
                            operation.Operands.Add(new ScriptOpOperand(Reader.PeekNullTerminatedString(Reader.ReadUInt16())));
                        }

                        break;
                    }
                case ScriptOperandType.SwitchEnd:
                    {
                        var switches = LoadEndSwitch();

                        foreach(var switchBlock in switches)
                        {
                            operation.Operands.Add(new ScriptOpOperand(switchBlock));
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid Op Type", "OpType");
                    }
            }

            operation.OpCodeSize = (int)Reader.BaseStream.Position - offset;

            return operation;
        }

        public override int GetJumpLocation(int from, int to)
        {
            return from + to;
        }

        public override List<ScriptOpSwitch> LoadEndSwitch()
        {
            List<ScriptOpSwitch> switches = new List<ScriptOpSwitch>();
            Reader.BaseStream.Position += Utility.ComputePadding((int)Reader.BaseStream.Position, 4);
            var switchCount = Reader.ReadInt32();

            for (int i = 0; i < switchCount; i++)
            {
                var switchValue = Reader.ReadUInt16();
                var flags = Reader.ReadUInt16();
                string switchString;

                if (flags == 0 && switchValue > 0)
                {
                    switchString = "\"" + Reader.PeekNullTerminatedString(switchValue) + "\"";
                }
                else if(flags == 0x80)
                {
                    switchString = switchValue.ToString();
                }
                else
                {
                    switchString = "default";
                }

                switches.Add(new ScriptOpSwitch()
                {
                    CaseValue = switchString,
                    ByteCodeOffset = (int)Reader.BaseStream.Position + Reader.ReadInt32() + 4,
                    OriginalIndex = i
                });
            }


            return switches.OrderBy(x => x.ByteCodeOffset).ToList();
        }

        public override void LoadAnimTrees()
        {
            Reader.BaseStream.Position = Header.AnimTreeTableOffset;

            AnimTrees = new List<ScriptAnimTree>(Header.AnimTreeCount);

            for (int i = 0; i < Header.AnimTreeCount; i++)
            {
                var nameOffset   = Reader.ReadUInt16();
                var refCount     = Reader.ReadUInt16();
                var animRefCount = Reader.ReadUInt16();
                Reader.BaseStream.Position += 2;

                var animTree = new ScriptAnimTree()
                {
                    Name                = Reader.PeekNullTerminatedString(nameOffset),
                    Offset              = nameOffset,
                    References          = new List<int>(refCount),
                    AnimationReferences = new List<ScriptAnim>(animRefCount),
                };

                for (int j = 0; j < refCount; j++)
                {
                    animTree.References.Add(Reader.ReadInt32());
                }

                for (int j = 0; j < animRefCount; j++)
                {
                    var animNameOffset = Reader.ReadInt32();
                    var refOffset = Reader.ReadInt32();
                    animTree.AnimationReferences.Add(new ScriptAnim()
                    {
                        Name      = Reader.PeekNullTerminatedString(animNameOffset),
                        Offset    = animNameOffset,
                        Reference = refOffset
                    });
                }

                AnimTrees.Add(animTree);
            }
        }
    }
}

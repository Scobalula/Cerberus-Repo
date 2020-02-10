using System;
using System.Collections.Generic;
using System.IO;
using PhilLibX.IO;
using System.Security.Cryptography;
using System.IO.Compression;

namespace Cerberus.Logic
{
    public static class FastFile
    {
        /// <summary>
        /// Invalid Characters from C# Reference Source
        /// </summary>
        internal static readonly char[] InvalidPathChars =
        {
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31
        };

        /// <summary>
        /// Black Ops II Fast File Decryption Key
        /// </summary>
        private static readonly byte[] FastFileKey =
        {
            0x64, 0x1D, 0x8A, 0x2F, 0xE3, 0x1D, 0x3A, 0xA6, 0x36, 0x22, 0xBB, 0xC9, 0xCE,
            0x85, 0x87, 0x22, 0x9D, 0x42, 0xB0, 0xF8, 0xED, 0x9B, 0x92, 0x41, 0x30, 0xBF,
            0x88, 0xB6, 0x5E, 0xDC, 0x50, 0xBE
        };

        /// <summary>
        /// Black Ops III Fast File Search Needle
        /// </summary>
        private static readonly byte[] NeedleBo3 = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Black Ops III Fast File Search Needle
        /// </summary>
        private static readonly byte[] NeedleBo2 = { 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Decodes Deflate byte array to Memory Stream
        /// </summary>
        /// <param name="data">Byte Array of Deflate Data</param>
        /// <returns>Decoded Memory Stream</returns>
        public static MemoryStream Decode(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            MemoryStream input = new MemoryStream(data);

            using (DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(output);
            }

            output.Flush();
            output.Position = 0;

            return output;
        }

        public static List<string> Decompress(string filePath, string outputPath)
        {
            Func<BinaryReader, List<string>> extractMethod = null;

            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            using(var writer = new BinaryWriter(File.Create(outputPath)))
            {
                var magic = reader.ReadUInt64();
                var version = reader.ReadUInt32();

                if(magic != 0x3030303066664154 && magic != 0x3030317566664154 && magic != 0x3030313066664154)
                {
                    throw new Exception("Invalid Fast File Magic.");
                }

                switch(version)
                {
                    case 0x251:
                        DecompressBO3(reader, writer);
                        extractMethod = ExtractScriptsBo3;
                        break;
                    case 0x93:
                        DecompressBO2(reader, writer);
                        extractMethod = ExtractScriptsBo2;
                        break;
                    default:
                        throw new Exception("Invalid Fast File Version.");
                }
            }

            using(var reader = new BinaryReader(File.OpenRead(outputPath)))
            {
                return extractMethod?.Invoke(reader);
            }
        }

        /// <summary>
        /// Decompresses a Black Ops III Fast File
        /// </summary>
        private static void DecompressBO3(BinaryReader reader, BinaryWriter writer)
        {
            var flags = reader.ReadBytes(4);

            // Validate the flags, we only support ZLIB, PC, and Non-Encrypted FFs
            if (flags[1] != 1)
            {
                throw new Exception("Invalid Fast File Compression. Only ZLIB Fast Files are supported.");
            }
            if (flags[2] != 0)
            {
                throw new Exception("Invalid Fast File Platform. Only PC Fast Files are supported.");
            }
            if (flags[3] != 0)
            {
                throw new Exception("Encrypted Fast Files are not supported");
            }

            reader.BaseStream.Position = 144;

            var size = reader.ReadInt64();
            var consumed = 0;

            reader.BaseStream.Position = 584;

            while(consumed < size)
            {
                // Read Block Header
                var compressedSize   = reader.ReadInt32();
                var decompressedSize = reader.ReadInt32();
                var blockSize        = reader.ReadInt32();
                var blockPosition    = reader.ReadInt32();

                // Validate the block position, it should match
                if(blockPosition != reader.BaseStream.Position - 16)
                {
                    throw new Exception("Block Position does not match Stream Position.");
                }

                // Check for padding blocks
                if(decompressedSize == 0)
                {
                    reader.BaseStream.Position += Utility.ComputePadding((int)reader.BaseStream.Position, 0x800000);
                    continue;
                }

                reader.BaseStream.Position += 2;
                writer.Write(Decode(reader.ReadBytes(compressedSize - 2)).ToArray());

                consumed += decompressedSize;

                // Sinze Fast Files are aligns, we must skip the full block
                reader.BaseStream.Position = blockPosition + 16 + blockSize;
            }
        }

        /// <summary>
        /// Extracts scripts from a Black Ops III Fast File
        /// </summary>
        private static List<string> ExtractScriptsBo3(BinaryReader reader)
        {
            // Need to skip the strings and assets
            // to avoid redundant checks on these by
            // the scanner
            var stringCount = reader.ReadInt32();
            reader.BaseStream.Position = 32;
            var assetCount = reader.ReadInt32();

            reader.BaseStream.Position = 56 + (stringCount - 1) * 8;

            for(int i = 0; i < stringCount; i++)
            {
                reader.ReadNullTerminatedString();
            }

            reader.BaseStream.Position += 16 * assetCount;


            var results = new List<string>();
            var offsets = reader.FindBytes(NeedleBo3);

            foreach(var offset in offsets)
            {
                try
                {
                    reader.BaseStream.Position = offset;

                    var namePtr = reader.ReadUInt64();
                    var size = reader.ReadInt64();
                    var dataPtr = reader.ReadUInt64();

                    // Check the pointers
                    if (namePtr == 0xFFFFFFFFFFFFFFFF && dataPtr == 0xFFFFFFFFFFFFFFFF && size <= uint.MaxValue)
                    {
                        // Linker only allows names up to 127
                        var name = reader.ReadNullTerminatedString(128);

                        if (name.IndexOfAny(InvalidPathChars) < 0)
                        {
                            var extension = Path.GetExtension(name);

                            // Last check, extension
                            if (extension == ".gsc" || extension == ".csc")
                            {
                                var outputPath = "ExtractedScripts\\Black Ops III\\" + name + "c";
                                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                                File.WriteAllBytes(outputPath, reader.ReadBytes((int)size));

                                results.Add(outputPath);
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Decompresses a Black Ops II Fast File
        /// </summary>
        private static void DecompressBO2(BinaryReader reader, BinaryWriter writer)
        {
            reader.BaseStream.Position += 12;

            var ivTable = new byte[16000];
            var ivCounter = new int[4];

            FillIVTable(ivTable, reader.ReadBytes(0x20));
            SetupIVCounter(ivCounter);

            reader.BaseStream.Position += 0x100;

            int sectionIndex = 0;
            var salsa = new Salsa20 { Key = FastFileKey };

            while (true)
            {
                int size = reader.ReadInt32();

                if (size == 0)
                    break;

                salsa.IV = GetIV(sectionIndex % 4, ivTable, ivCounter);

                var decryptor = salsa.CreateDecryptor();

                byte[] decryptedData = decryptor.TransformFinalBlock(reader.ReadBytes(size), 0, size);

                writer.Write(Decode(decryptedData).ToArray());

                using (var sha1 = SHA1.Create())
                {
                    UpdateIVTable(sectionIndex % 4, sha1.ComputeHash(decryptedData), ivTable, ivCounter);
                }

                sectionIndex++;
            }
        }

        /// <summary>
        /// Extracts scripts from a Black Ops II Fast File
        /// </summary>
        private static List<string> ExtractScriptsBo2(BinaryReader reader)
        {
            // Need to skip the strings and assets
            // to avoid redundant checks on these by
            // the scanner
            reader.BaseStream.Position = 40;
            var stringCount = reader.ReadInt32();
            reader.BaseStream.Position = 56;
            var assetCount = reader.ReadInt32();

            reader.BaseStream.Position = 68 + stringCount * 4;

            for (int i = 0; i < stringCount; i++)
            {
                reader.ReadNullTerminatedString();
            }

            reader.BaseStream.Position += 8 * assetCount;


            var results = new List<string>();
            var offsets = reader.FindBytes(NeedleBo2);

            foreach (var offset in offsets)
            {
                try
                {
                    reader.BaseStream.Position = offset;

                    var namePtr = reader.ReadUInt32();
                    var size = reader.ReadInt32();
                    var dataPtr = reader.ReadUInt32();

                    // Check the pointers
                    if (namePtr == 0xFFFFFFFF && dataPtr == 0xFFFFFFFF && size <= int.MaxValue)
                    {
                        // Linker only allows names up to 127
                        var name = reader.ReadNullTerminatedString(128);

                        if (name.IndexOfAny(InvalidPathChars) < 0)
                        {
                            var extension = Path.GetExtension(name);

                            // Last check, extension
                            if (extension == ".gsc" || extension == ".csc")
                            {
                                var outputPath = "ExtractedScripts\\BlackOps II\\" + name + "c";
                                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                                File.WriteAllBytes(outputPath, reader.ReadBytes((int)size));

                                results.Add(outputPath);
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Updates IV Table
        /// </summary>
        private static void UpdateIVTable(int index, byte[] hash, byte[] ivTable, int[] ivCounter)
        {
            for (int i = 0; i < 20; i += 5)
            {
                int value = (index + 4 * ivCounter[index]) % 800 * 5;
                for (int x = 0; x < 5; x++)
                    ivTable[4 * value + x + i] ^= hash[i + x];
            }
            ivCounter[index]++;
        }

        /// <summary>
        /// Gets the IV
        /// </summary>
        private static byte[] GetIV(int index, byte[] ivTable, int[] ivCounter)
        {
            var iv = new byte[8];
            int arrayIndex = (index + 4 * (ivCounter[index] - 1)) % 800 * 20;
            Array.Copy(ivTable, arrayIndex, iv, 0, 8);
            return iv;
        }

        /// <summary>
        /// Sets up IV Counter
        /// </summary>
        private static void SetupIVCounter(int[] ivCounter)
        {
            for (int i = 0; i < 4; i++)
                ivCounter[i] = 1;
        }

        /// <summary>
        /// Fills IV Table
        /// </summary>
        private static void FillIVTable(byte[] ivTable, byte[] nameKey)
        {
            int nameKeyLength = Array.FindIndex(nameKey, b => b == 0);

            int addDiv = 0;
            for (int i = 0; i < ivTable.Length; i += nameKeyLength * 4)
            {
                for (int x = 0; x < nameKeyLength * 4; x += 4)
                {
                    if ((i + addDiv) >= ivTable.Length || i + x >= ivTable.Length)
                        return;

                    if (x > 0)
                        addDiv = x / 4;
                    else
                        addDiv = 0;

                    for (int y = 0; y < 4; y++)
                        ivTable[i + x + y] = nameKey[addDiv];
                }
            }
        }
    }
}

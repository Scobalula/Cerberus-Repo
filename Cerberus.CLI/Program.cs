using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cerberus.Logic;
using CommandLine;
using CommandLine.Text;

namespace Cerberus.CLI
{
    class Program
    {
        /// <summary>
        /// File Extensions we accept
        /// </summary>
        static readonly string[] AcceptedExtensions =
        {
            ".ff",
            ".gsc",
            ".csc",
            ".gscc",
            ".cscc",
        };

        /// <summary>
        /// Directory of the processed scripts
        /// </summary>
        static readonly string ProcessDirectory = "ProcessedScripts";

        /// <summary>
        /// Command Line Options
        /// </summary>
        static CliOptions Options { get; set; }

        /// <summary>
        /// Supported Hash Tables
        /// </summary>
        static readonly Dictionary<string, Dictionary<uint, string>> HashTables = new Dictionary<string, Dictionary<uint, string>>()
        {
            { "BlackOps2", new Dictionary<uint, string>() },
            { "BlackOps3", new Dictionary<uint, string>() },
        };

        /// <summary>
        /// Class to hold CLI options
        /// </summary>
        class CliOptions
        {
            [Option('v', "verbose", Required = false, HelpText = "Outputs more information to the console.")]
            public bool Verbose { get; set; }
            [Option('d', "disassemble", Required = false, HelpText = "Disassembles the script/s.")]
            public bool Disassemble { get; set; }
            [Option('n', "close", Required = false, HelpText = "Closes the program once execution has finished.")]
            public bool Close { get; set; }
            [Option('h', "help", Required = false, HelpText = "Prints this message.")]
            public bool Help { get; set; }
        }

        /// <summary>
        /// Loads in required hash tables
        /// </summary>
        static void LoadHashTables()
        {
            PrintVerbose(": Loading hash tables...");
            foreach (var hashTable in HashTables)
            {
                try
                {
                    string[] lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), hashTable.Key + ".txt"));
                    PrintVerbose(": Loading " + hashTable.Key + ".txt");
                    foreach (string line in lines)
                    {
                        try
                        {
                            string lineTrim = line.Trim();

                            // Ignore comment lines
                            if (!lineTrim.StartsWith("#"))
                            {
                                string[] lineSplit = lineTrim.Split(',');

                                if (lineSplit.Length > 1)
                                {
                                    // Parse as hex, without 0x
                                    if (uint.TryParse(lineSplit[0].TrimStart('0', 'x'), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var hash))
                                    {
                                        hashTable.Value[hash] = lineSplit[1];
                                    }
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Prints a message in verbose mode
        /// </summary>
        static void PrintVerbose(object value)
        {
            if(Options?.Verbose == true)
            {
                Console.WriteLine(value);
            }
        }

        /// <summary>
        /// Prints help output
        /// </summary>
        static void PrintHelp(ParserResult<CliOptions> outputOptions)
        {
            var helpText = new HelpText
            {
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
            };

            helpText.AddOptions(outputOptions);

            var stuff = helpText.ToString().Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));

            Console.WriteLine(": Example: Cerberus.CLI [options] <files (.gsc|.csc|.gscc|.cscc|.ff)>");
            Console.WriteLine(": Options: ");

            foreach (var item in stuff)
            {
                Console.WriteLine(":\t{0}", item.Trim());
            }
        }

        /// <summary>
        /// Processes a script file
        /// </summary>
        /// <param name="filePath"></param>
        static void ProcessScript(string filePath)
        {
            using(var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                using (var script = ScriptBase.LoadScript(reader, HashTables))
                {
                    PrintVerbose(string.Format(": Processing {0} script.", script.Game));
                    var outputPath = Path.Combine(ProcessDirectory, script.Game, script.FilePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    PrintVerbose(string.Format(": Outputting to {0}", outputPath));

                    if (Options.Disassemble)
                    {
                        PrintVerbose(": Disassembling script..");
                        File.WriteAllText(outputPath + ".script_asm" + Path.GetExtension(outputPath), script.Disassemble());
                    }

                    PrintVerbose(": Decompiling script..");
                    File.WriteAllText(outputPath + ".decompiled" + Path.GetExtension(outputPath), script.Decompile());
                }
            }
        }

        /// <summary>
        /// Main Entry Point
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine(": ----------------------------------------------------------");
            Console.WriteLine(": Cerberus Command Line - Black Ops II/III Script Decompiler");
            Console.WriteLine(": Developed by Scobalula");
            Console.WriteLine(": Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(": ----------------------------------------------------------");

            var parser = new Parser(config => config.HelpWriter = null);
            var cliOptions = parser.ParseArguments<CliOptions>(args).WithParsed(x => Options = x).WithNotParsed(_ => Options = new CliOptions());

            var filesProcessed = 0;

            // Force working directory back to exe
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Console.WriteLine(": Exporting to: {0}", Directory.GetCurrentDirectory());

            LoadHashTables();

            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.*", SearchOption.AllDirectories);

            Console.WriteLine(files.Length);

            foreach (var arg in files)
            {
                try
                {
                    if (AcceptedExtensions.Contains(Path.GetExtension(arg).ToLower()))
                    {
                        if (File.Exists(arg))
                        {
                            filesProcessed++;
                            Console.WriteLine(": Processing {0}...", Path.GetFileName(arg));

                            switch (Path.GetExtension(arg).ToLower())
                            {
                                case ".gsc":
                                case ".csc":
                                case ".gscc":
                                case ".cscc":
                                    {
                                        ProcessScript(arg);
                                        break;
                                    }
                            }

                            Console.WriteLine(": Processed {0} successfully.", Path.GetFileName(arg));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(": An error has occured while processing {0}: {1}", Path.GetFileName(arg), e.Message);
                    PrintVerbose(e);
                }
            }

            //            foreach (var arg in args)
            //            {
            //                try
            //                {
            //                    if (AcceptedExtensions.Contains(Path.GetExtension(arg).ToLower()))
            //                    {
            //                        if (File.Exists(arg))
            //                        {
            //                            filesProcessed++;
            //                            Console.WriteLine(": Processing {0}...", Path.GetFileName(arg));

            //                            switch (Path.GetExtension(arg).ToLower())
            //                            {
            //                                case ".gsc":
            //                                case ".csc":
            //                                case ".gscc":
            //                                case ".cscc":
            //                                    {
            //                                        ProcessScript(arg);
            //                                        break;
            //                                    }
            //                                case ".ff":
            //                                    {
            //                                        PrintVerbose(": Decompressing and Processing Fast File.....");

            //                                        // Skip ZM Temple, it causes issues due to a weird large number
            //                                        // of blocks
            //                                        if (Path.GetFileName(arg) != "zm_temple")
            //                                        {
            //                                            try
            //                                            {
            //                                                var files = FastFile.Decompress(arg, arg + ".output");

            //                                                if (Options.Verbose)
            //                                                {
            //                                                    foreach (var file in files)
            //                                                    {
            //                                                        Console.WriteLine(": Found {0}", file);
            //                                                    }
            //                                                }

            //                                            }
            //                                            catch (Exception e)
            //                                            {
            //                                                PrintVerbose(e);
            //                                                throw e;
            //                                            }
            //                                            finally
            //                                            {
            //#if !DEBUG
            //                                                File.Delete(arg + ".output");
            //#endif
            //                                            }
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            Console.WriteLine(": Processed {0} successfully.", Path.GetFileName(arg));
            //                        }
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    Console.WriteLine(": An error has occured while processing {0}: {1}", Path.GetFileName(arg), e.Message);
            //                    PrintVerbose(e);
            //                }
            //            }

            if (Options.Help || filesProcessed <= 0)
            {
                PrintHelp(cliOptions);
            }

            GC.Collect();

            if (Options.Close == false)
            {
                Console.WriteLine(": Execution completed successfully, press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
// ------------------------------------------------------------------------
// Cerberus - A Call of Duty: Black Ops II/III GSC/CSC Decompiler
// Copyright (C) 2018 Philip/Scobalula
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General private License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General private License for more details.

// You should have received a copy of the GNU General private License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;

namespace Cerberus.Logic
{
    /// <summary>
    /// Handles Decompiling GSC files
    /// </summary>
    internal class Decompiler : IDisposable
    {
        /// <summary>
        /// Operators by Op Code
        /// </summary>
        static readonly Dictionary<ScriptOpCode, string> Operators = new Dictionary<ScriptOpCode, string>()
        {
            { ScriptOpCode.Plus,                          " + " },
            { ScriptOpCode.Minus,                         " - " },
            { ScriptOpCode.Multiply,                      " * " },
            { ScriptOpCode.Divide,                        " / " },
            { ScriptOpCode.Modulus,                       " % " },
            { ScriptOpCode.ShiftLeft,                     " << " },
            { ScriptOpCode.ShiftRight,                    " >> " },
            { ScriptOpCode.Bit_Or,                        " | " },
            { ScriptOpCode.Bit_Xor,                       " ^ " },
            { ScriptOpCode.Bit_And,                       " & " },
            { ScriptOpCode.Equal,                         " == " },
            { ScriptOpCode.NotEqual,                      " != " },
            { ScriptOpCode.LessThan,                      " < " },
            { ScriptOpCode.GreaterThan,                   " > " },
            { ScriptOpCode.LessThanOrEqualTo,             " <= " },
            { ScriptOpCode.GreaterThanOrEqualTo,          " >= " },
            { ScriptOpCode.SuperEqual,                    " === " },
            { ScriptOpCode.SuperNotEqual,                 " !== " },
        };

        /// <summary>
        /// Instructions that have a function to match
        /// </summary>
        static readonly Dictionary<ScriptOpCode, Tuple<string, int>> InstructionFunctions = new Dictionary<ScriptOpCode, Tuple<string, int>>()
        {
            // Op Code                                                             "Source Name"            "Parameter Count"
            { ScriptOpCode.RealWait,                        new Tuple<string, int>("realwait",              1) },
            { ScriptOpCode.Wait,                            new Tuple<string, int>("wait",                  1) },
            { ScriptOpCode.WaitRealTime,                    new Tuple<string, int>("WaitRealTime",            1) },
            { ScriptOpCode.GetTime,                         new Tuple<string, int>("GetTime",               0) },
            { ScriptOpCode.Abs,                             new Tuple<string, int>("Abs",                   1) },
            { ScriptOpCode.FirstArrayKey,                   new Tuple<string, int>("GetFirstArrayKey",      1) },
            { ScriptOpCode.NextArrayKey,                    new Tuple<string, int>("GetNextArrayKey",       2) },
            { ScriptOpCode.AnglesToUp,                      new Tuple<string, int>("AnglesToUp",            1) },
            { ScriptOpCode.AnglesToRight,                   new Tuple<string, int>("AnglesToRight",         1) },
            { ScriptOpCode.AnglesToForward,                 new Tuple<string, int>("AnglesToForward",       1) },
            { ScriptOpCode.AngleClamp180,                   new Tuple<string, int>("AngleClamp180",         1) },
            { ScriptOpCode.VectorToAngles,                  new Tuple<string, int>("VectorToAngles",        1) },
            { ScriptOpCode.VectorScale,                     new Tuple<string, int>("VectorScale",           2) },
            { ScriptOpCode.IsDefined,                       new Tuple<string, int>("isdefined",             1) },
            { ScriptOpCode.GetDvar,                         new Tuple<string, int>("GetDvar",               1) },
            { ScriptOpCode.GetDvarInt,                      new Tuple<string, int>("GetDvarInt",            1) },
            { ScriptOpCode.GetDvarFloat,                    new Tuple<string, int>("GetDvarFloat",          1) },
            { ScriptOpCode.GetDvarVector,                   new Tuple<string, int>("GetDvarVector",         1) },
            { ScriptOpCode.GetDvarColorRed,                 new Tuple<string, int>("GetDvarColorRed",       1) },
            { ScriptOpCode.GetDvarColorGreen,               new Tuple<string, int>("GetDvarColorGreen",     1) },
            { ScriptOpCode.GetDvarColorBlue,                new Tuple<string, int>("GetDvarColorBlue",      1) },
            { ScriptOpCode.GetDvarColorAlpha,               new Tuple<string, int>("GetDvarColorAlpha",     1) },
        };

        /// <summary>
        /// Gets or Sets the Function we're decompiling
        /// </summary>
        private ScriptExport Function { get; set; }

        /// <summary>
        /// Gets or Sets the Script the function belongs to
        /// </summary>
        private ScriptBase Script { get; set; }

        /// <summary>
        /// List of decompiler blocks
        /// </summary>
        private List<DecompilerBlock> Blocks = new List<DecompilerBlock>();

        /// <summary>
        /// List of local variable names
        /// </summary>
        private readonly List<string> LocalVariables = new List<string>();

        /// <summary>
        /// The virtual script stack
        /// </summary>
        private readonly Stack<string> Stack = new Stack<string>();

        /// <summary>
        /// Current Variable Reference
        /// </summary>
        private string CurrentReference = "";

        /// <summary>
        /// Current Object
        /// </summary>
        private string CurrentObject = "";

        /// <summary>
        /// Internal Text Writer
        /// </summary>
        private StringWriter InternalWriter { get; set; }

        /// <summary>
        /// Indent Writer
        /// </summary>
        private IndentedTextWriter Writer { get; set; }

        /// <summary>
        /// Initializes an instance of the Decompiler Class
        /// </summary>
        /// <param name="function"></param>
        /// <param name="script"></param>
        public Decompiler(ScriptExport function, ScriptBase script)
        {
            try
            {
                Function = function;
                Script = script;

                // Preprocess some operations
                foreach (var operation in Function.Operations)
                {
                    if (operation.Metadata.OpCode == ScriptOpCode.SafeCreateLocalVariables)
                    {
                        foreach (var var in operation.Operands)
                        {
                            LocalVariables.Add((string)var.Value);
                        }

                        operation.Visited = true;
                    }

                    // Check for invalid operations
                    if (operation.Metadata.OpCode == ScriptOpCode.Invalid)
                    {
                        throw new Exception("Function contains invalid operation code");
                    }
                }

                // Remove end instruction if it's end,
                if (Function.Operations[Function.Operations.Count - 1].Metadata.OpCode == ScriptOpCode.End)
                {
                    Function.Operations[Function.Operations.Count - 1].Visited = true;
                }

                // Add the root of this function, the main block of execution
                Blocks.Add(new BasicBlock(function.ByteCodeOffset, function.ByteCodeOffset + function.ByteCodeSize + 1));

                // This performs several passes over the operations
                // we detect the most basic and easy to find first
                // then we move onto transforming them into their
                // respective parents

                // We need to find jump blocks before 
                // we find for loops as we can have a case
                // where we have modifiers within an if/else
                // and therefore it's NOT a for loop
                // but we need to resolve for loops before them
                // so this is the best way to ensure that
                FindSwitchCase();
                FindDevBlocks();
                FindWhileLoops();
                FindDoWhileLoops();
                FindIfStatements();
                FindJumpBlocks();
                // FindForEachLoops();
                // FindForLoops();
                // Now that we've done what need to do we can remove jump blocks
                // to process them properly
                Blocks.RemoveAll(x => x is BasicBlock && x.StartOffset != Function.ByteCodeOffset);
                FindElseIfStatements();
                ResolveParentBlocks();
                Stack.Clear();
                RestoreCaseOrder();

                InternalWriter = new StringWriter();
                Writer = new IndentedTextWriter(InternalWriter, "\t");

                Writer.WriteLine(BuildFunctionDefinition());
                DecompileBlock(Blocks[0], 1);
            }
            catch(Exception e)
            {
                InternalWriter?.Dispose();
                InternalWriter = new StringWriter();
                Writer?.Dispose();
                Writer = new IndentedTextWriter(InternalWriter, "\t");
                Writer.WriteLine("function {0}()", Function.Name);
                Writer.WriteLine("{");
                Writer.WriteLine(e.ToString().Trim());
                Writer.WriteLine("}");
            }

            Writer.Flush();
            ClearVisitedInstructions();
        }

        public string GetWriterOutput()
        {
            return InternalWriter.ToString();
        }

        /// <summary>
        /// Builds the function definition
        /// </summary>
        private string BuildFunctionDefinition()
        {
            var result = "function ";

            // Flags, currently only 2 are known (Private/Autoexec)
            if(Function.Flags.HasFlag(ScriptExportFlags.Private))
            {
                result += "private ";
            }
            if (Function.Flags.HasFlag(ScriptExportFlags.AutoExec))
            {
                result += "autoexec ";
            }

            result += Function.Name + "(";

            // Build paramters (TODO: Some checks for default args, it'll be an isdefined check after the call)
            for (int i = 0; i < Function.ParameterCount; i++)
            {
                result += string.Format("{0}{1}", LocalVariables[i], i == Function.ParameterCount - 1 ? "" : ", ");
            }

            return result + ")";
        }

        /// <summary>
        /// Clears Visited Instructions
        /// </summary>
        private void ClearVisitedInstructions()
        {
            Function.Operations.ForEach(x => x.Visited = false);
        }

        /// <summary>
        /// Restores the original order of the case block
        /// 
        /// For child-block resolving purposes we sort the blocks
        /// based off their offsets, etc. but then the order of their
        /// execution is wrong
        /// 
        /// To fix this we store the original index and then restore it
        /// here
        /// </summary>
        private void RestoreCaseOrder()
        {
            foreach(var block in Blocks)
            {
                if(block is SwitchBlock caseBlock)
                {
                    block.ChildBlockIndices = block.ChildBlockIndices.OrderBy(x => ((CaseBlock)Blocks[x]).OriginalIndex).ToList();
                }
            }
        }

        private void FindJumpBlocks()
        {
            foreach(var instruction in Function.Operations)
            {
                if(instruction.Metadata.OpCode == ScriptOpCode.Jump && instruction.Visited == false)
                {
                    // Check positive jump
                    if((int)instruction.Operands[0].Value > 0)
                    {

                        // Add it as a basic block
                        Blocks.Add(new BasicBlock(
                            instruction.OpCodeOffset + instruction.OpCodeSize,
                            Script.GetJumpLocation(
                                instruction.OpCodeOffset + instruction.OpCodeSize,
                                (int)instruction.Operands[0].Value)));
                    }
                }
            }
        }

        private void FindIfStatements()
        {
            for (int i = 0; i < Function.Operations.Count; i++)
            {
                var op = Function.Operations[i];

                if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                {
                    if (!op.Visited && (int)op.Operands[0].Value > 0)
                    {
                        op.Visited = true;

                        var startIndex = FindStartIndexEx(i - 1);

                        Blocks.Add(new IfBlock(Function.Operations[startIndex].OpCodeOffset, Script.GetJumpLocation(op.OpCodeOffset + op.OpCodeSize, (int)op.Operands[0].Value))
                        {
                            Comparison = BuildCondition(startIndex)
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the instruction is inside a child block
        /// 
        /// This is used by the for loop finder to determine 
        /// if an index modifier, etc. is actually within another
        /// block
        /// </summary>
        private bool IsInsideChildBlock(int offset, DecompilerBlock block)
        {
            for(int i = Blocks.Count - 1; i >= 0; i--)
            {
                if (Blocks[i].StartOffset > block.StartOffset && 
                    Blocks[i].EndOffset <= block.EndOffset)
                {
                    if (offset >= Blocks[i].StartOffset && offset < Blocks[i].EndOffset)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool LoopHasReferences(WhileLoop loop)
        {
            foreach(var op in Function.Operations)
            {
                if(!op.Visited)
                {
                    if(op.Metadata.OpCode == ScriptOpCode.Jump)
                    {
                        var jumpLocation = Script.GetJumpLocation(
                                op.OpCodeOffset + op.OpCodeSize,
                                (int)op.Operands[0].Value);

                        if(jumpLocation == loop.ContinueOffset)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void FindElseIfStatements()
        {
            // TODO: Improved If/else if detection, this seems to do a good job though for now
            // Ideally I'd want this to traverse the block to ensure the else if is actually
            // what we want, reason being is what if we have a if within the else that's at the
            // start?

            for (int i = 1; i < Blocks.Count; i++)
            {
                if (Blocks[i] is IfBlock || Blocks[i] is ElseIfBlock)
                {
                    var index = GetInstructionAt(Blocks[i].EndOffset);

                    if(Function.Operations[index - 1].Metadata.OpCode == ScriptOpCode.Jump)
                    {
                        var op = Function.Operations[index - 1];

                        // Attempt to locate an else if, otherwise we're using this as the jump
                        var blockIndex = GetBlockIndexAt(Blocks[i].EndOffset);
                        var jumpLocation = Script.GetJumpLocation(
                                op.OpCodeOffset + op.OpCodeSize,
                                (int)op.Operands[0].Value);

                        if (!IsContinue(op.OpCodeOffset, jumpLocation) && !IsBreak(op.OpCodeOffset, jumpLocation))
                        {
                            Function.Operations[index - 1].Visited = true;

                            if (blockIndex > 0 && Blocks[blockIndex] is IfBlock elseIf && jumpLocation != Blocks[i].EndOffset)
                            {
                                // Mark this block
                                Blocks[blockIndex] = new ElseIfBlock(elseIf.StartOffset, elseIf.EndOffset)
                                {
                                    Comparison = elseIf.Comparison
                                };
                            }
                            else
                            {
                                // Mark this block
                                Blocks.Add(new ElseBlock(
                                    op.OpCodeOffset + op.OpCodeSize,
                                Script.GetJumpLocation(
                                    op.OpCodeOffset + op.OpCodeSize,
                                    (int)op.Operands[0].Value)));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Locates DoWhile Loops
        /// 
        /// They're actually quite simple, they are simply negative jump statements
        /// So we can consider a non-visited JumpOn with a negative jump to be a DoWhile(...)
        /// </summary>
        private void FindDoWhileLoops()
        {
            for(int i = 0; i < Function.Operations.Count; i++)
            {
                var op = Function.Operations[i];

                if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                {
                    if(!op.Visited && (int)op.Operands[0].Value < 0)
                    {
                        op.Visited = true;
                        Blocks.Add(new DoWhileLoop(Script.GetJumpLocation(op.OpCodeOffset + op.OpCodeSize, (int)op.Operands[0].Value), op.OpCodeOffset)
                        {
                            Comparison = BuildCondition(FindStartIndexEx(i - 1))
                        });
                    }
                }
            }
        }
        private void FindSwitchCase()
        {
            foreach (var operation in Function.Operations)
            {
                if (operation.Metadata.OpCode == ScriptOpCode.Switch)
                {
                    var switchBlock = new SwitchBlock(
                        operation.OpCodeOffset, 
                        Script.GetJumpLocation(
                            operation.OpCodeOffset + operation.OpCodeSize, 
                            (int)operation.Operands[0].Value));
                    Blocks.Add(switchBlock);

                    Script.Reader.BaseStream.Position = switchBlock.EndOffset;
                    var cases = Script.LoadEndSwitch();

                    for(int i = 0; i < cases.Count; i++)
                    {
                        var block = cases[i];
                        var caseBlock = new CaseBlock(block.ByteCodeOffset, 0)
                        {
                            Value = block.CaseValue,
                            OriginalIndex = block.OriginalIndex
                        };

                        // We can either use this block, or the main switch, as our end offset
                        if(i == cases.Count - 1)
                        {
                            caseBlock.EndOffset = switchBlock.EndOffset;
                        }
                        else
                        {
                            // Use the next block
                            caseBlock.EndOffset = cases[i + 1].ByteCodeOffset;
                        }

                        Blocks.Add(caseBlock);
                    }

                    switchBlock.BreakOffset = switchBlock.EndOffset + (cases.Count * 8) + 4;
                }
            }
        }

        private void ResolveParentBlocks()
        {
            // Sort it
            Blocks = Blocks.OrderBy(x => x.StartOffset).ToList();

            for(int i = Blocks.Count - 1; i >= 0; i--)
            {
                for(int j = Blocks.Count - 1; j >= 0; j--)
                {
                    if (Blocks[j] is CaseBlock && !(Blocks[i] is SwitchBlock) && !(Blocks[i] is CaseBlock))
                    {
                        if (Blocks[i].StartOffset >= Blocks[j].StartOffset && Blocks[i].EndOffset <= Blocks[j].EndOffset)
                        {
                            Blocks[j].ChildBlockIndices.Add(i);
                            break;
                        }
                    }
                    else if(Blocks[i] is CaseBlock && !(Blocks[j] is SwitchBlock))
                    {
                        continue;
                    }
                    else
                    {
                        if (Blocks[i].StartOffset > Blocks[j].StartOffset && Blocks[i].EndOffset <= Blocks[j].EndOffset)
                        {
                            Blocks[j].ChildBlockIndices.Add(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a source function call (i.e. Func(p1, p2, p3))
        /// 
        /// Method means we're calling on something
        /// </summary>
        private string GenerateFunctionCall(string functionName, int paramCount, bool threaded, bool method)
        {
            string result = "";

            if(method)
            {
                result += Stack.Pop() + " ";
            }

            if(threaded)
            {
                result += "thread ";
            }

            result += functionName + "(";

            for(int i = 0; i < paramCount; i++)
            {
                result += Stack.Pop();

                if(i != paramCount - 1)
                {
                    result += ", ";
                }
            }

            result += ")";

            return result;
        }

        private void WriteHeader(DecompilerBlock block)
        {
            var size = block.EndOffset - block.StartOffset;

            if (!string.IsNullOrWhiteSpace(block.GetHeader()))
            {
                Writer?.WriteLine(block.GetHeader());
            }

            if(block.RequiresBraces)
            {
                if (size <= 0 && block.CanSkipZeroSize)
                {
                    return;
                }

                Writer?.WriteLine("{");
            }
        }

        private void WriteFooter(DecompilerBlock block)
        {
            var size = block.EndOffset - block.StartOffset;

            if (block.RequiresBraces)
            {
                if (size <= 0 && block.CanSkipZeroSize)
                {
                    return;
                }

                Writer?.WriteLine("}");
            }

            if (!string.IsNullOrWhiteSpace(block.GetFooter()))
            {
                Writer?.WriteLine(block.GetFooter());
            }
        }

        private bool IsContinue(int ip, int offset)
        {
            return Blocks.FindIndex(x => (x.StartOffset >= offset && x.EndOffset < offset) && x.ContinueOffset == offset && (x.StartOffset >= ip && x.EndOffset < ip)) >= 0;
        }

        private bool IsBreak(int ip, int offset)
        {
            return Blocks.FindIndex(x => (x.StartOffset >= offset && x.EndOffset < offset) && x.BreakOffset == offset && (x.StartOffset >= ip && x.EndOffset < ip)) >= 0;
        }

        private void DecompileBlock(DecompilerBlock decompilerBlock, int tabs)
        {
            if(decompilerBlock is SwitchBlock switchBlock)
            {
                switchBlock.Value = Stack.Pop();
            }

            WriteHeader(decompilerBlock);
            Writer.Indent = tabs;

            for(int i = GetInstructionAt(decompilerBlock.StartOffset); i < Function.Operations.Count && Function.Operations[i].OpCodeOffset <= decompilerBlock.EndOffset; i++)
            {
                var operation = Function.Operations[i];

                if (operation.Metadata.OpCode == ScriptOpCode.Invalid)
                {
                    throw new Exception("Function contains invalid OpCode.");
                }

                foreach (var childIndex in decompilerBlock.ChildBlockIndices)
                {
                    if(Blocks[childIndex].StartOffset == operation.OpCodeOffset)
                    {
                        DecompileBlock(Blocks[childIndex], tabs + 1);
                    }
                }

                if(operation.Visited)
                {
                    continue;
                }

                Function.Operations[i].Visited = true;

                // Pass to processor
                ProcessInstruction(operation);
            }
            Writer.Indent = tabs - 1;
            WriteFooter(decompilerBlock);
        }

        /// <summary>
        /// Locates Developer Blocks
        /// </summary>
        void FindDevBlocks()
        {
            foreach(var operation in Function.Operations)
            {
                if(operation.Metadata.OpCode == ScriptOpCode.DevblockBegin)
                {
                    // Dev Blocks are simple, just a size
                    Blocks.Add(new DevBlock(operation.OpCodeOffset, Script.GetJumpLocation(operation.OpCodeOffset + operation.OpCodeSize, (int)operation.Operands[0].Value)));
                }
            }
        }

        /// <summary>
        /// Builds a basic list of blocks
        /// 
        /// As per usual, any jump we can consider to start and end a block
        /// However, GSC, being as high level as it is, contains much more info
        /// we can use.
        /// Examples are switch statements and dev blocks. The real trouble comes with
        /// determining loops and if/else, which itself is quite easy, on the loop side
        /// however we need to be able to identify foreach and for loops, along with 
        /// ternary
        /// We can consider a ternary operator to be a if statement that has 1 stack 
        /// instruction, but this is more data anaylsis than control flow
        /// For loops we can check initialization, comparison, and incrementation,
        /// along with checking jump statements within if statements, as they can 
        /// jump onto the incrementer
        /// </summary>
        private void FindWhileLoops()
        {
            // We perform a reverse scan, since we're looking for negative jumps
            // If we come across a negative jump it can be one of 2 things: a continue
            // statement, or a loop. We can determine if it's a continue if we can find
            // the jump location within our list of blocks, if we can't, it's a new loop

            // We don't check for break statements here as we first need to ensure we've
            // resolved for loops, etc. later
            for(int i = Function.Operations.Count - 1; i >= 0; i--)
            {
                switch(Function.Operations[i].Metadata.OpCode)
                {
                    case ScriptOpCode.Jump:
                        {
                            // Check for a negative jumps, is almost always a loop
                            if((int)Function.Operations[i].Operands[0].Value < 0)
                            {
                                // Compute the jump location based as some games align the value
                                var offset = Script.GetJumpLocation(
                                    Function.Operations[i].OpCodeOffset + Function.Operations[i].OpCodeSize, 
                                    (int)Function.Operations[i].Operands[0].Value);

                                // Attempt to locate the block at the current location, if we fail we're
                                // creating a new loop, otherwise this is probably continue statement
                                if(!IsContinue(Function.Operations[i].OpCodeOffset, offset))
                                {
                                    Function.Operations[i].Visited = true;

                                    // Attempt to resolve is this a for(;;) or a while(...)
                                    bool hasCondition = false;
                                    var end = Function.Operations[i].OpCodeOffset;
                                    var conditionStart = GetInstructionAt(offset);

                                    // Keep going until we either hit 
                                    for (int j = conditionStart; j < Function.Operations.Count; j++)
                                    {
                                        var op = Function.Operations[j];

                                        // Check if we hit a JumpOn, if we do, we're probably a while(...)
                                        if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                                        {
                                            hasCondition = true;
                                            break;
                                        }

                                        // Check if we hit a non-stack/expression operation
                                        if (!IsStackOperation(op) && op.Metadata.OpType != ScriptOpType.JumpExpression)
                                        {
                                            break;
                                        }
                                    }

                                    if (hasCondition)
                                    {
                                        // We have a condition, add as a while
                                        var whileLoop = new WhileLoop(offset, end)
                                        {
                                            Comparison = BuildCondition(conditionStart),
                                            ContinueOffset = offset,
                                            BreakOffset = end + Function.Operations[i].OpCodeSize
                                        };

                                        Blocks.Add(whileLoop);
                                    }
                                    else
                                    {
                                        // We're an infinite for(;;)
                                        Blocks.Add(new ForEver(offset, end)
                                        {
                                            ContinueOffset = end
                                        });
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Builds an expression jump
        /// </summary>
        private string BuildExpression(ScriptOp startOp)
        {
            // JumpOnTrue is || JumpOnFalse is &&
            var requiresBraces = false;
            var endOffset = Script.GetJumpLocation(startOp.OpCodeOffset + startOp.OpCodeSize, (int)startOp.Operands[0].Value);

            var startIndex = GetInstructionAt(startOp.OpCodeOffset + startOp.OpCodeSize);
            var endIndex = GetInstructionAt(endOffset);

            // Attempt to build it
            for (int j = startIndex; j < endIndex; j++)
            {
                var op = Function.Operations[j];

                // Check if we hit a non-stack/expression operation
                if (!IsStackOperation(op))
                {
                    throw new Exception("Unexpected non-stack operation within jump expression");
                }

                if (op.Visited == true)
                {
                    continue;
                }

                if (op.Metadata.OpType == ScriptOpType.JumpExpression)
                {
                    requiresBraces = true;
                }

                ProcessInstruction(op);
                op.Visited = true;
            }

            var result = Stack.Pop();

            // Determine if it needs braces (nested expressions)
            if (requiresBraces)
                result = "(" + result + ")";

            return result;
        }

        private string BuildCondition(int startIndex)
        {
            var result = "";
            var requiresBraces = false;

            for(int j = startIndex; j < Function.Operations.Count; j++)
            {
                var op = Function.Operations[j];

                if (op.Metadata.OpType == ScriptOpType.JumpExpression)
                {
                    requiresBraces = true;
                }

                if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                {
                    result += Stack.Pop();
                    // If it's a forward JumpOnTrue, we add ! since the block won't execute if the
                    // the condition is true, however for dowhile this will be JumpOnTrue because
                    // we want to continue with the execution (back to start) if it's true
                    if(op.Metadata.OpCode == ScriptOpCode.JumpOnTrue && (int)op.Metadata.OpCode > 0)
                    {
                        if(requiresBraces)
                        {
                            result = "(" + result + ")";
                        }

                        result = "!" + result;
                    }

                    op.Visited = true;
                    break;
                }

                if (op.Visited)
                {
                    continue;
                }

                // Check if we hit a non-stack/expression operation
                if (!IsStackOperation(op))
                {
                    break;
                }

                ProcessInstruction(op);
                op.Visited = true;
            }

            return result;
        }

        /// <summary>
        /// Locates ForEach loops
        /// 
        /// A explicit call to First/NextArrayKey via an instruction is always going
        /// to be foreach, attempting to call GetFirstArrayKey in GSC will generate the
        /// call, where as foreach will generate the instruction
        /// </summary>
        private void FindForEachLoops()
        {
            for (int i = 0; i < Function.Operations.Count; i++)
            {
                var op = Function.Operations[i];

                if(op.Metadata.OpCode == ScriptOpCode.FirstArrayKey)
                {
                    // Validate them
                    if (
                        Function.Operations[i - 3].Metadata.OpCode == ScriptOpCode.EvalLocalVariableRefCached &&
                        Function.Operations[i - 2].Metadata.OpCode == ScriptOpCode.SetVariableField &&
                        Function.Operations[i - 1].Metadata.OpCode == ScriptOpCode.EvalLocalVariableCached
                        )
                    {
                        var index = GetBlockIndexAt(Function.Operations[i + 3].OpCodeOffset);

                        if(Blocks[index] is WhileLoop loop)
                        {
                            // Attempt to resolve info
                            var variableBeginIndex = FindStartIndex(i - 4);
                            int continueOffset;

                            // Process so we can obtain the real variable name
                            for (int j = variableBeginIndex; j < i - 3; j++)
                            {
                                ProcessInstruction(Function.Operations[j]);
                                Function.Operations[j].Visited = true;
                            }

                            // Mark these operations
                            Function.Operations[i - 3].Visited = true;
                            Function.Operations[i - 2].Visited = true;
                            Function.Operations[i - 1].Visited = true;
                            Function.Operations[i + 0].Visited = true;
                            Function.Operations[i + 1].Visited = true;
                            Function.Operations[i + 2].Visited = true;
                            Function.Operations[i + 3].Visited = true;
                            Function.Operations[i + 4].Visited = true;
                            Function.Operations[i + 5].Visited = true;
                            Function.Operations[i + 6].Visited = true;
                            Function.Operations[i + 7].Visited = true;
                            Function.Operations[i + 8].Visited = true;
                            Function.Operations[i + 9].Visited = true;
                            Function.Operations[i + 10].Visited = true;

                            // Clear the instructions at the end
                            var opIndex = GetInstructionAt(loop.EndOffset);

                            // Now that we've determined it's a foreach, we need to remove the necessary
                            // operations, along with determining the continue location, since Bo3 and 2 
                            // are slightly different
                            if (Function.Operations[i + 13].Metadata.OpCode == ScriptOpCode.NextArrayKey)
                            {
                                Function.Operations[i + 11].Visited = true;
                                Function.Operations[i + 12].Visited = true;
                                Function.Operations[i + 13].Visited = true;
                                Function.Operations[i + 14].Visited = true;
                                Function.Operations[i + 15].Visited = true;
                                Function.Operations[opIndex - 1].Visited = true;
                                Function.Operations[opIndex - 2].Visited = true;
                                Function.Operations[opIndex - 3].Visited = true;
                                continueOffset = Function.Operations[opIndex - 3].OpCodeOffset;
                            }
                            else
                            {
                                Function.Operations[opIndex - 1].Visited = true;
                                Function.Operations[opIndex - 2].Visited = true;
                                Function.Operations[opIndex - 3].Visited = true;
                                Function.Operations[opIndex - 4].Visited = true;
                                Function.Operations[opIndex - 5].Visited = true;
                                continueOffset = Function.Operations[opIndex - 5].OpCodeOffset;
                            }

                            Blocks[index] = new ForEach(loop.StartOffset, loop.EndOffset)
                            {
                                ArrayName = Stack.Pop(),
                                IteratorName = GetVariableName(Function.Operations[i + 9]),
                                ContinueOffset = continueOffset,
                                BreakOffset = loop.BreakOffset
                            };
                        }
                        else
                        {
                            throw new ArgumentException("Expecting While Loop At FirstArrayKey");
                        }
                    }
                }
            }
        }

        private int GetBlockForInstruction(ScriptOp op)
        {
            for(int i = Blocks.Count - 1; i >= 0; i--)
            {
                if(op.OpCodeOffset >= Blocks[i].StartOffset && op.OpCodeOffset < Blocks[i].EndOffset)
                {
                    return i;
                }
            }

            return -1;
        }

        private void FindForLoops()
        {
            // TODO: Optimize this A LOT, make it better (so it doesn't fall back to shitty checks), etc.
            // It does the job for now, and does it pretty well, but it can be better, and doesn't handle
            // out of the ordinary cases (but I've never ran into such)

            // The idea is I want to reduce it and make it better at detecting shit

            for(int i = 0; i < Blocks.Count; i++)
            {
                var block = Blocks[i];

                if(block is WhileLoop whileLoop)
                {
                    if (!LoopHasReferences(whileLoop))
                    {
                        var index = GetInstructionAt(whileLoop.StartOffset);

                        // For now we just check for Variable Reference + SetVariableField, a comparison, and a increment
                        if (
                            Function.Operations[index - 1].Metadata.OpCode == ScriptOpCode.SetVariableField &&
                            Function.Operations[index - 2].Metadata.OpType == ScriptOpType.VariableReference)
                        {
                            bool isCompared = false;

                            // Attempt to resolve info
                            var variableBeginIndex = FindStartIndex(index - 2, true);

                            // Validate if it's inside a previous block
                            var blockIndex = GetBlockForInstruction(Function.Operations[variableBeginIndex]);

                            if(blockIndex > -1 && blockIndex != i)
                            {
                                if (whileLoop.StartOffset >= Blocks[blockIndex].EndOffset && whileLoop.StartOffset >= Blocks[blockIndex].StartOffset)
                                {
                                    continue;
                                }
                            }

                            // Process so we can obtain the real variable name
                            for (int j = variableBeginIndex; j < index; j++)
                            {
                                ProcessInstruction(Function.Operations[j]);
                            }

                            // First let's see if this is the variable we can use
                            var variableName = CurrentReference;

                            // Attempt to hit a comparison to this
                            for (int j = index; j < Function.Operations.Count; j++)
                            {
                                
                                var op = Function.Operations[j];

                                // Check if we hit a JumpOn..etc. we're done
                                if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                                {
                                    break;
                                }

                                // Check if we hit a non-stack/expression operation
                                if (!IsStackOperation(op) && op.Metadata.OpType != ScriptOpType.JumpExpression)
                                {
                                    break;
                                }

                                if (op.Metadata.OpType == ScriptOpType.Variable)
                                {
                                    var compVar = GetVariableName(op);

                                    // At some point, this variable is being compared
                                    // So we can try and use it
                                    if (compVar == variableName)
                                    {
                                        isCompared = true;
                                        break;
                                    }
                                }
                            }

                            // Check if we hit a valid comparison
                            if (isCompared)
                            {
                                // Now let's do a backwards scan like the while, until we hit a reference to the variable, if we do
                                // We can be farily confident it's a for loop and can mark is as such
                                // We can also run a jump check again to see forward jumps that match us
                                var endIndex = GetInstructionAt(whileLoop.EndOffset) - 2;
                                bool isModified = false;
                                int referenceIndex = -1;

                                // Attempt to hit a comparison to this
                                for (int j = endIndex; j >= 0; j--)
                                {
                                    var op = Function.Operations[j];

                                    if (IsInsideChildBlock(op.OpCodeOffset, whileLoop))
                                    {
                                        break;
                                    }

                                    if (op.Metadata.OpType == ScriptOpType.JumpCondition)
                                    {
                                        break;
                                    }

                                    if (op.Metadata.OpType == ScriptOpType.SingleOperand)
                                    {
                                        continue;
                                    }

                                    if (op.OpCodeOffset < whileLoop.StartOffset)
                                    {
                                        continue;
                                    }

                                    if (op.Metadata.OpType == ScriptOpType.VariableReference)
                                    {
                                        var compVar = GetVariableName(op);

                                        // At some point, this variable is being compared
                                        // So we can try and use it
                                        if (compVar == variableName)
                                        {
                                            referenceIndex = j;
                                            isModified = true;
                                            break;
                                        }
                                    }

                                    if (!IsStackOperation(op))
                                    {
                                        break;
                                    }
                                }

                                // Final straw, IS SHE MODIFIED
                                if (isModified)
                                {

                                    // First let's build the intializer
                                    var initBegin = FindStartIndex(index - 2, true);

                                    var forLoop = new ForLoopBlock(whileLoop.StartOffset, whileLoop.EndOffset)
                                    {
                                        Comparison = whileLoop.Comparison,
                                        BreakOffset = whileLoop.BreakOffset,
                                    };

                                    for (int j = initBegin; j < index; j++)
                                    {
                                        // We've now visited/processed this
                                        Function.Operations[j].Visited = true;
                                        var op = Function.Operations[j];



                                        if (op.Metadata.OpType == ScriptOpType.SetVariable)
                                        {
                                            forLoop.Initializer = string.Format("{0} = {1}", CurrentReference, Stack.Pop());
                                            CurrentReference = "";
                                            break;
                                        }


                                        ProcessInstruction(op);
                                    }

                                    int modifierStart;

                                    // Last we build the modifier
                                    // First we can check if it's a simple case of op inc

                                    if (
                                        Function.Operations[referenceIndex + 1].Metadata.OpCode == ScriptOpCode.Dec ||
                                        Function.Operations[referenceIndex + 1].Metadata.OpCode == ScriptOpCode.Inc
                                        )
                                    {
                                        modifierStart = referenceIndex;
                                    }
                                    else
                                    {
                                        // Resolve it
                                        modifierStart = FindStartIndex(referenceIndex) - 1;
                                    }

                                    forLoop.ContinueOffset = Function.Operations[modifierStart].OpCodeOffset;

                                    for (int j = modifierStart; j < Function.Operations.Count && Function.Operations[j].OpCodeOffset < whileLoop.EndOffset; j++)
                                    {
                                        // We've now visited/processed this
                                        Function.Operations[j].Visited = true;
                                        var op = Function.Operations[j];

                                        if (FoundForLoopModifier(op, forLoop))
                                        {
                                            break;
                                        }
                                    }

                                    Blocks[i] = forLoop;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool FoundForLoopModifier(ScriptOp op, ForLoopBlock loop)
        {
            switch (op.Metadata.OpType)
            {
                case ScriptOpType.SetVariable:
                    {
                        loop.Modifier = string.Format("{0} = {1}", CurrentReference, Stack.Pop());
                        CurrentReference = "";
                        return true;
                    }
                case ScriptOpType.SingleOperand:
                    {
                        switch (op.Metadata.OpCode)
                        {
                            case ScriptOpCode.Dec:
                                loop.Modifier = string.Format("{0}--", CurrentReference);
                                return true;
                            case ScriptOpCode.Inc:
                                loop.Modifier = string.Format("{0}++", CurrentReference);
                                return true;
                            case ScriptOpCode.Bit_Not:
                                loop.Modifier = string.Format("~{0}", CurrentReference);
                                return true;
                        }

                        return false;
                    }
                default:
                    ProcessInstruction(op);
                    return false;
            }
        }

        /// <summary>
        /// Attempts to find the start of the given statement, etc. by looping until we hit 
        /// a non-stack operation
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int FindStartIndexEx(int index)
        {
            // Attempt to hit a comparison to this
            for (int j = index; j >= 0; j--)
            {
                var op = Function.Operations[j];

                // Check if we hit a non-stack/expression operation
                if (IsStackOperation(op) || op.Metadata.OpType == ScriptOpType.JumpExpression)
                {
                    continue;
                }

                // We add 1 since we want the next operation, this one is not what we want
                return j + 1;
            }

            return -1;
        }

        /// <summary>
        /// Attempts to find the start of the given statement, etc. by looping until we hit 
        /// a non-stack operation
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int FindStartIndex(int index, bool referenceAllowed = false)
        {
            // Attempt to hit a comparison to this
            for (int j = index; j >= 0; j--)
            {
                var op = Function.Operations[j];

                // For some statements we can count the reference
                if(referenceAllowed && op.Metadata.OpType == ScriptOpType.VariableReference)
                {
                    continue;
                }

                // Check if we hit a non-stack/expression operation
                if (IsStackOperation(op) || op.Metadata.OpType == ScriptOpType.JumpExpression)
                {
                    continue;
                }

                // We add 1 since we want the next operation, this one is not what we want
                return j + 1;
            }

            return -1;
        }

        /// <summary>
        /// Attempts to find the start of the given statement, etc. by looping until we hit 
        /// a non-stack operation
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int FindLoopModifierBegin(int index)
        {
            // Attempt to hit a comparison to this
            for (int j = index; j >= 0; j--)
            {
                var op = Function.Operations[j];

                // Check if we hit a non-stack/expression operation
                if (IsStackOperation(op) && op.Metadata.OpType == ScriptOpType.JumpExpression)
                {
                    continue;
                }

                // We add 1 since we want the next operation, this one is not what we want
                return j - 1;
            }

            return -1;
        }

        private string GetLocalVariable(int index)
        {
            return LocalVariables[LocalVariables.Count + ~index];
        }

        private string GetVariableName(ScriptOp op)
        {
            switch(op.Metadata.OpCode)
            {
                case ScriptOpCode.EvalLocalVariableCached:
                case ScriptOpCode.EvalLocalVariableRefCached:
                case ScriptOpCode.SetWaittillVariableFieldCached:
                    return GetLocalVariable((int)op.Operands[0].Value);
                case ScriptOpCode.EvalFieldVariable:
                case ScriptOpCode.EvalFieldVariableRef:
                    return CurrentObject + "." + (string)op.Operands[0].Value;
                case ScriptOpCode.EvalLevelFieldVariable:
                case ScriptOpCode.EvalLevelFieldVariableRef:
                    return "level." + (string)op.Operands[0].Value;
                case ScriptOpCode.EvalSelfFieldVariable:
                case ScriptOpCode.EvalSelfFieldVariableRef:
                    return "level." + (string)op.Operands[0].Value;
                default:
                    throw new ArgumentException("Invalid Op Code for GetVariableName");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetBlockIndexAt(int offset)
        {
            return Blocks.FindIndex(x => x.StartOffset == offset);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetInstructionAt(int offset)
        {
            return Function.Operations.FindIndex(x => x.OpCodeOffset == offset);
        }

        private bool IsStackOperation(ScriptOp op)
        {
            if (op.Metadata.OpType == ScriptOpType.StackPush ||
                op.Metadata.OpType == ScriptOpType.Variable ||
                op.Metadata.OpType == ScriptOpType.Call ||
                op.Metadata.OpType == ScriptOpType.Array ||
                op.Metadata.OpType == ScriptOpType.DoubleOperand ||
                op.Metadata.OpType == ScriptOpType.Comparison ||
                op.Metadata.OpType == ScriptOpType.SizeOf ||
                op.Metadata.OpType == ScriptOpType.JumpExpression ||
                op.Metadata.OpCode == ScriptOpCode.PreScriptCall ||
                op.Metadata.OpCode == ScriptOpCode.SizeOf ||
                op.Metadata.OpCode == ScriptOpCode.BoolNot ||
                op.Metadata.OpType == ScriptOpType.Object

                )
            {
                if (op.Metadata.OpCode != ScriptOpCode.Wait &&
                    op.Metadata.OpCode != ScriptOpCode.WaitRealTime)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Processes the given instructions and returns if the instruction
        /// exits the current block
        /// </summary>
        private bool ProcessInstruction(ScriptOp operation, DecompilerBlock block = null)
        {
            if(operation.Metadata.OpCode == ScriptOpCode.Invalid)
            {
                throw new Exception("Function contains invalid OpCode.");
            }

            switch (operation.Metadata.OpType)
            {
                case ScriptOpType.Return:
                    {
                        if (operation.Metadata.OpCode == ScriptOpCode.End)
                        {
                            Writer?.WriteLine("return;");

                        }
                        else
                        {
                            Writer?.WriteLine("return {0};", Stack.Pop());
                        }

                        return false;
                    }
                case ScriptOpType.StackPop:
                    {
                        Writer?.WriteLine("{0};", Stack.Pop());
                        break;
                    }
                case ScriptOpType.SizeOf:
                    {
                        Stack.Push(Stack.Pop() + ".size");
                        break;
                    }
                case ScriptOpType.Jump:
                    {
                        var jumpLoc = Script.GetJumpLocation(
                            operation.OpCodeOffset + operation.OpCodeSize,
                            (int)operation.Operands[0].Value);

                        if (IsBreak(operation.OpCodeOffset, jumpLoc))
                        {
                            Writer?.WriteLine("break;");
                        }
                        else
                        {
                            Writer?.WriteLine("continue;");
                        }

                        return false;
                    }
                case ScriptOpType.JumpExpression:
                    {
                        Stack.Push(Stack.Pop() + (operation.Metadata.OpCode == ScriptOpCode.JumpOnFalseExpr ? " && " : " || ") + BuildExpression(operation));
                        break;
                    }
                case ScriptOpType.ObjectReference:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.GetGameRef:
                                CurrentReference = "game";
                                break;
                        }
                        break;
                    }
                case ScriptOpType.StackPush:
                    {
                        // If we have no operands we shall push what needs to be pushed
                        if (operation.Metadata.OperandType == ScriptOperandType.None)
                        {
                            // For these we just need to manually give it
                            // what the source equivilent would be
                            switch (operation.Metadata.OpCode)
                            {
                                case ScriptOpCode.GetUndefined:
                                    Stack.Push("undefined");
                                    break;
                                case ScriptOpCode.GetZero:
                                    Stack.Push("0");
                                    break;
                                case ScriptOpCode.GetSelf:
                                    Stack.Push("self");
                                    break;
                                case ScriptOpCode.GetLevel:
                                    Stack.Push("level");
                                    break;
                                case ScriptOpCode.GetGame:
                                    Stack.Push("game");
                                    break;
                                case ScriptOpCode.GetAnim:
                                    Stack.Push("anim");
                                    break;
                                case ScriptOpCode.GetWorld:
                                    Stack.Push("world");
                                    break;
                                case ScriptOpCode.GetEmptyArray:
                                    Stack.Push("[]");
                                    break;
                                case ScriptOpCode.Vector:
                                    Stack.Push(string.Format("({0}, {1}, {2})", Stack.Pop(), Stack.Pop(), Stack.Pop()));
                                    break;
                            }
                        }
                        else
                        {
                            switch (operation.Metadata.OperandType)
                            {
                                case ScriptOperandType.FunctionPointer:
                                    {
                                        var import = Script.GetImport(operation.OpCodeOffset);

                                        var functionName = import.Name;

                                        // Check if we can omit the namespace, if it's the same as this, otherwise we need to add it
                                        if (!string.IsNullOrWhiteSpace(import.Namespace) && import.Namespace != Function.Namespace)
                                        {
                                            functionName = import.Namespace + "::" + functionName;
                                        }

                                        Stack.Push("&" + functionName);
                                        break;
                                    }
                                default:
                                    {
                                        // We have a value
                                        Stack.Push(operation.Operands[0].Value.ToString());
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case ScriptOpType.Object:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.GetSelfObject:
                                CurrentObject = "self";
                                break;
                            case ScriptOpCode.GetLevelObject:
                                CurrentObject = "level";
                                break;
                            case ScriptOpCode.GetAnimObject:
                                CurrentObject = "anim";
                                break;
                            case ScriptOpCode.GetWorldObject:
                                CurrentObject = "world";
                                break;
                            case ScriptOpCode.GetClassesObject:
                                CurrentObject = "classes";
                                break;
                            case ScriptOpCode.CastFieldObject:
                                CurrentObject = Stack.Pop();
                                break;
                        }
                        break;
                    }
                case ScriptOpType.ClearVariable:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.ClearFieldVariable:
                                {
                                    Writer?.WriteLine("{0}.{1} = undefined;", CurrentObject, operation.Operands[0].Value);
                                    break;
                                }
                            case ScriptOpCode.ClearArray:
                                {
                                    Writer?.WriteLine("{0}[{1}] = undefined;", CurrentReference, Stack.Pop());
                                    CurrentReference = "";
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.Comparison:
                    {
                        var right = Stack.Pop();
                        var left = Stack.Pop();
                        Stack.Push(string.Format("{0}{1}{2}", left, Operators[operation.Metadata.OpCode], right));
                        break;
                    }
                case ScriptOpType.DoubleOperand:
                    {
                        var right = Stack.Pop();
                        var left = Stack.Pop();
                        Stack.Push(string.Format("{0}{1}{2}", left, Operators[operation.Metadata.OpCode], right));
                        break;
                    }
                case ScriptOpType.SingleOperand:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.Dec:
                                {
                                    Writer?.WriteLine("" + CurrentReference + "--;");
                                    CurrentReference = "";
                                    break;
                                }
                            case ScriptOpCode.Inc:
                                {
                                    Writer?.WriteLine("" + CurrentReference + "++;");
                                    CurrentReference = "";
                                    break;
                                }
                            case ScriptOpCode.Bit_Not:
                                {
                                    Writer?.WriteLine("~" + CurrentReference + ";");
                                    CurrentReference = "";
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.Cast:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.BoolNot:
                                {
                                    var value = Stack.Pop();

                                    if(value.Contains("&&") || value.Contains("||"))
                                    {
                                        value = "(" + value + ")";
                                    }

                                    Stack.Push(string.Format("!{0}", value));
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.Call:
                    {
                        // Store here as we'll resolve the method type
                        string functionName;
                        int paramCount;
                        bool threaded = false;
                        bool method = false;

                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.ScriptFunctionCallPointer:
                            case ScriptOpCode.ScriptMethodCallPointer:
                            case ScriptOpCode.ScriptThreadCallPointer:
                            case ScriptOpCode.ScriptMethodThreadCallPointer:
                                {
                                    // Pointers are wrapped
                                    functionName = "[[" + Stack.Pop() + "]]";
                                    paramCount = (int)operation.Operands[0].Value;

                                    // Check for thread calls
                                    if (
                                        operation.Metadata.OpCode == ScriptOpCode.ScriptThreadCallPointer ||
                                        operation.Metadata.OpCode == ScriptOpCode.ScriptMethodThreadCallPointer)
                                    {
                                        threaded = true;
                                    }

                                    // Check for method calls
                                    if (
                                    operation.Metadata.OpCode == ScriptOpCode.ScriptMethodCallPointer ||
                                    operation.Metadata.OpCode == ScriptOpCode.ScriptMethodThreadCallPointer)
                                    {
                                        method = true;
                                    }
                                    break;
                                }
                            case ScriptOpCode.ScriptFunctionCall:
                            case ScriptOpCode.ScriptMethodCall:
                            case ScriptOpCode.ScriptMethodThreadCall:
                            case ScriptOpCode.ScriptThreadCall:
                                {
                                    var functionImport = Script.GetImport(operation.OpCodeOffset);

                                    functionName = functionImport.Name;
                                    paramCount = functionImport.ParameterCount;

                                    // Check if we can omit the namespace, if it's the same as this, otherwise we need to add it
                                    if (!string.IsNullOrWhiteSpace(functionImport.Namespace) && functionImport.Namespace != Function.Namespace)
                                    {
                                        functionName = functionImport.Namespace + "::" + functionName;
                                    }

                                    // Check for thread calls
                                    if (
                                        operation.Metadata.OpCode == ScriptOpCode.ScriptThreadCall ||
                                        operation.Metadata.OpCode == ScriptOpCode.ScriptMethodThreadCall)
                                    {
                                        threaded = true;
                                    }

                                    // Check for method calls
                                    if (
                                    operation.Metadata.OpCode == ScriptOpCode.ScriptMethodCall ||
                                    operation.Metadata.OpCode == ScriptOpCode.ScriptMethodThreadCall)
                                    {
                                        method = true;
                                    }
                                    break;
                                }
                            case ScriptOpCode.ClassFunctionCall:
                                {
                                    functionName = (string)operation.Operands[0].Value;
                                    paramCount = (int)operation.Operands[1].Value;
                                    break;
                                }
                            default:
                                {
                                    // Everything else take from the instruction table
                                    var opFunc = InstructionFunctions[operation.Metadata.OpCode];
                                    functionName = opFunc.Item1;
                                    paramCount = opFunc.Item2;
                                    break;
                                }
                        }

                        // Push the call as it's basically a stack item
                        // wait is not pushed, it's technically not a call
                        if (operation.Metadata.OpCode == ScriptOpCode.Wait || operation.Metadata.OpCode == ScriptOpCode.WaitRealTime)
                        {
                            Writer?.WriteLine("{0};", GenerateFunctionCall(functionName, paramCount, threaded, method));
                        }
                        else
                        {
                            Stack.Push(GenerateFunctionCall(functionName, paramCount, threaded, method));
                        }

                        break;
                    }
                case ScriptOpType.Notification:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.EndOn:
                                {
                                    Writer?.WriteLine("{0} endon({1});", Stack.Pop(), Stack.Pop());
                                    break;
                                }
                            case ScriptOpCode.Notify:
                                {
                                    Writer?.Write("{0} notify({1}", Stack.Pop(), Stack.Pop());

                                    while (Stack.Count > 0)
                                    {
                                        Writer.Write(", {0}", Stack.Pop());
                                    }

                                    Writer.WriteLine(");");

                                    break;
                                }
                            case ScriptOpCode.WaitTill:
                            case ScriptOpCode.WaitTillMatch:
                                {
                                    Writer.Write("{0} {1}({2}", Stack.Pop(), operation.Metadata.OpCode == ScriptOpCode.WaitTill ? "waittill" : "waittillmatch", Stack.Pop());

                                    // Parse the variables created by a waittill
                                    var index = GetInstructionAt(operation.OpCodeOffset) + 1;
                                    while(Function.Operations[index].Metadata.OpCode == ScriptOpCode.SetWaittillVariableFieldCached)
                                    {
                                        Writer.Write(", {0}", GetVariableName(Function.Operations[index]));
                                        index++;
                                    }

                                    Writer.WriteLine(");");

                                    break;
                                }
                            case ScriptOpCode.WaitTillFrameEnd:
                                {
                                    Writer.WriteLine("waittillframeend;");
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.Variable:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.EvalLocalVariableCached:
                                {
                                    Stack.Push(LocalVariables[LocalVariables.Count + ~(int)operation.Operands[0].Value]);
                                    break;
                                }
                            case ScriptOpCode.EvalFieldVariable:
                                {
                                    Stack.Push(CurrentObject + "." + (string)operation.Operands[0].Value);
                                    break;
                                }
                            // Black Ops 3 merges level/self eval into 1
                            case ScriptOpCode.EvalLevelFieldVariable:
                                {
                                    Stack.Push("level." + (string)operation.Operands[0].Value);
                                    break;
                                }
                            case ScriptOpCode.EvalSelfFieldVariable:
                                {
                                    Stack.Push("self." + (string)operation.Operands[0].Value);
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.VariableReference:
                    {
                        switch (operation.Metadata.OpCode)
                        {
                            case ScriptOpCode.EvalLocalVariableRefCached:
                                {
                                    CurrentReference = LocalVariables[LocalVariables.Count - (int)operation.Operands[0].Value - 1];
                                    break;
                                }
                            case ScriptOpCode.EvalFieldVariableRef:
                                {
                                    CurrentReference = CurrentObject + "." + (string)operation.Operands[0].Value;
                                    break;
                                }
                            // Black Ops 3 merges level/self eval into 1
                            case ScriptOpCode.EvalLevelFieldVariableRef:
                                {
                                    CurrentReference = "level." + (string)operation.Operands[0].Value;
                                    break;
                                }
                            case ScriptOpCode.EvalSelfFieldVariableRef:
                                {
                                    CurrentReference = "self." + (string)operation.Operands[0].Value;
                                    break;
                                }
                        }
                        break;
                    }
                case ScriptOpType.Array:
                    {
                        var var = Stack.Pop();
                        var key = Stack.Pop();
                        Stack.Push(var + "[" + key + "]");
                        break;
                    }
                case ScriptOpType.ArrayReference:
                    {
                        CurrentReference += "[" + Stack.Pop() + "]";
                        break;
                    }
                case ScriptOpType.SetVariable:
                    {
                        Writer?.WriteLine("{0} = {1};", CurrentReference, Stack.Pop());
                        break;
                    }
            }

            return true;
        }

        /// <summary>
        /// Disposes of the internal writers
        /// </summary>
        public void Dispose()
        {
            InternalWriter?.Dispose();
            Writer?.Dispose();
        }
    }
}

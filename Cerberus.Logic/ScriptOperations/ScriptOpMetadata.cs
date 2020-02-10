using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A static class to hold operation metadata
    /// </summary>
    public class ScriptOpMetadata
    {
        /// <summary>
        /// Script Operation Metadata for each operation type
        /// </summary>
        public static ScriptOpMetadata[] OperationInfo =
        {
            new ScriptOpMetadata(ScriptOpCode.End,                              ScriptOpType.Return,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Return,                           ScriptOpType.Return,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetUndefined,                     ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetZero,                          ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetByte,                          ScriptOpType.StackPush,         ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.GetNegByte,                       ScriptOpType.StackPush,         ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.GetUnsignedShort,                 ScriptOpType.StackPush,         ScriptOperandType.UInt16),
            new ScriptOpMetadata(ScriptOpCode.GetNegUnsignedShort,              ScriptOpType.StackPush,         ScriptOperandType.UInt16),
            new ScriptOpMetadata(ScriptOpCode.GetInteger,                       ScriptOpType.StackPush,         ScriptOperandType.Int32),
            new ScriptOpMetadata(ScriptOpCode.GetFloat,                         ScriptOpType.StackPush,         ScriptOperandType.Float),
            new ScriptOpMetadata(ScriptOpCode.GetString,                        ScriptOpType.StackPush,         ScriptOperandType.String),
            new ScriptOpMetadata(ScriptOpCode.GetIString,                       ScriptOpType.StackPush,         ScriptOperandType.String),
            new ScriptOpMetadata(ScriptOpCode.GetVector,                        ScriptOpType.StackPush,         ScriptOperandType.Vector),
            new ScriptOpMetadata(ScriptOpCode.GetLevelObject,                   ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetAnimObject,                    ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetSelf,                          ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetLevel,                         ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetGame,                          ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetAnim,                          ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetAnimation,                     ScriptOpType.StackPush,         ScriptOperandType.String),
            new ScriptOpMetadata(ScriptOpCode.GetGameRef,                       ScriptOpType.ObjectReference,   ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetFunction,                      ScriptOpType.StackPush,         ScriptOperandType.FunctionPointer),
            new ScriptOpMetadata(ScriptOpCode.CreateLocalVariable,              ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.SafeCreateLocalVariables,         ScriptOpType.None,              ScriptOperandType.VariableList),
            new ScriptOpMetadata(ScriptOpCode.RemoveLocalVariables,             ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLocalVariableCached,          ScriptOpType.Variable,          ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.EvalArray,                        ScriptOpType.Array,             ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLocalArrayRefCached,          ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalArrayRef,                     ScriptOpType.ArrayReference,    ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ClearArray,                       ScriptOpType.ClearVariable,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetEmptyArray,                    ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetSelfObject,                    ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalFieldVariable,                ScriptOpType.Variable,          ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.EvalFieldVariableRef,             ScriptOpType.VariableReference, ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.ClearFieldVariable,               ScriptOpType.ClearVariable,     ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.SafeSetVariableFieldCached,       ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.SetWaittillVariableFieldCached,   ScriptOpType.None,              ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.ClearParams,                      ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.CheckClearParams,                 ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLocalVariableRefCached,       ScriptOpType.VariableReference, ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.SetVariableField,                 ScriptOpType.SetVariable,       ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.CallBuiltin,                      ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.CallBuiltinMethod,                ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.Wait,                             ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.WaitTillFrameEnd,                 ScriptOpType.Notification,      ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.PreScriptCall,                    ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ScriptFunctionCall,               ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.ScriptFunctionCallPointer,        ScriptOpType.Call,              ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.ScriptMethodCall,                 ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.ScriptMethodCallPointer,          ScriptOpType.Call,              ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.ScriptThreadCall,                 ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.ScriptThreadCallPointer,          ScriptOpType.Call,              ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.ScriptMethodThreadCall,           ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.ScriptMethodThreadCallPointer,    ScriptOpType.Call,              ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.DecTop,                           ScriptOpType.StackPop,          ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.CastFieldObject,                  ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.CastBool,                         ScriptOpType.Cast,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.BoolNot,                          ScriptOpType.Cast,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.BoolComplement,                   ScriptOpType.Cast,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.JumpOnFalse,                      ScriptOpType.JumpCondition,     ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.JumpOnTrue,                       ScriptOpType.JumpCondition,     ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.JumpOnFalseExpr,                  ScriptOpType.JumpExpression,    ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.JumpOnTrueExpr,                   ScriptOpType.JumpExpression,    ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.Jump,                             ScriptOpType.Jump,              ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.Jump,                             ScriptOpType.Jump,              ScriptOperandType.Int16),
            new ScriptOpMetadata(ScriptOpCode.Inc,                              ScriptOpType.SingleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Dec,                              ScriptOpType.SingleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Bit_Or,                           ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Bit_Xor,                          ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Bit_And,                          ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Equal,                            ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.NotEqual,                         ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.LessThan,                         ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GreaterThan,                      ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.LessThanOrEqualTo,                ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GreaterThanOrEqualTo,             ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ShiftLeft,                        ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ShiftRight,                       ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Plus,                             ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Minus,                            ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Multiply,                         ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Divide,                           ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Modulus,                          ScriptOpType.DoubleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.SizeOf,                           ScriptOpType.SizeOf,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.WaitTillMatch,                    ScriptOpType.Notification,      ScriptOperandType.UInt8),
            new ScriptOpMetadata(ScriptOpCode.WaitTill,                         ScriptOpType.Notification,      ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Notify,                           ScriptOpType.Notification,      ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EndOn,                            ScriptOpType.Notification,      ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.VoidCodePos,                      ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Switch,                           ScriptOpType.Switch,            ScriptOperandType.Int32),
            new ScriptOpMetadata(ScriptOpCode.EndSwitch,                        ScriptOpType.SwitchCases,       ScriptOperandType.SwitchEnd),
            new ScriptOpMetadata(ScriptOpCode.Vector,                           ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetHash,                          ScriptOpType.StackPush,         ScriptOperandType.Hash),
            new ScriptOpMetadata(ScriptOpCode.RealWait,                         ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.VectorConstant,                   ScriptOpType.StackPush,         ScriptOperandType.VectorFlags),
            new ScriptOpMetadata(ScriptOpCode.IsDefined,                        ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.VectorScale,                      ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.AnglesToUp,                       ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.AnglesToRight,                    ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.AnglesToForward,                  ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.AngleClamp180,                    ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.VectorToAngles,                   ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Abs,                              ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetTime,                          ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvar,                          ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarInt,                       ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarFloat,                     ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarVector,                    ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarColorRed,                  ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarColorGreen,                ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarColorBlue,                 ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetDvarColorAlpha,                ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.FirstArrayKey,                    ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.NextArrayKey,                     ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ProfileStart,                     ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ProfileStop,                      ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.SafeDecTop,                       ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Nop,                              ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Abort,                            ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.Obj,                              ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ThreadObject,                     ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLocalVariable,                ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLocalVariableRef,             ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.DevblockBegin,                    ScriptOpType.None,              ScriptOperandType.UInt16),
            new ScriptOpMetadata(ScriptOpCode.DevblockEnd,                      ScriptOpType.None,              ScriptOperandType.UInt16),
            new ScriptOpMetadata(ScriptOpCode.Breakpoint,                       ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.AutoBreakpoint,                   ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ErrorBreakpoint,                  ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.WatchBreakpoint,                  ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.NotifyBreakpoint,                 ScriptOpType.None,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetObjectType,                    ScriptOpType.StackPush,         ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.WaitRealTime,                     ScriptOpType.Call,              ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetWorldObject,                   ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetClassesObject,                 ScriptOpType.Object,            ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.ClassFunctionCall,                ScriptOpType.Call,              ScriptOperandType.Call),
            new ScriptOpMetadata(ScriptOpCode.Bit_Not,                          ScriptOpType.SingleOperand,     ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.GetWorld,                         ScriptOpType.StackPush,         ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.EvalLevelFieldVariable,           ScriptOpType.Variable,          ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.EvalLevelFieldVariableRef,        ScriptOpType.VariableReference, ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.EvalSelfFieldVariable,            ScriptOpType.Variable,          ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.EvalSelfFieldVariableRef,         ScriptOpType.VariableReference, ScriptOperandType.VariableName),
            new ScriptOpMetadata(ScriptOpCode.SuperEqual,                       ScriptOpType.Comparison,        ScriptOperandType.None),
            new ScriptOpMetadata(ScriptOpCode.SuperNotEqual,                    ScriptOpType.Comparison,        ScriptOperandType.None),
        };

        /// <summary>
        /// Gets or Sets the Operation Code
        /// </summary>
        public ScriptOpCode OpCode { get; set; }

        /// <summary>
        /// Gets or Sets the Operation Type
        /// </summary>
        public ScriptOpType OpType { get; set; }

        /// <summary>
        /// Gets or Sets the Operand Data Type/s
        /// </summary>
        public ScriptOperandType OperandType { get; set; }

        /// <summary>
        /// Initializes and instance of the Metadata Class with the given info
        /// </summary>
        public ScriptOpMetadata(ScriptOpCode opCode, ScriptOpType opType, ScriptOperandType operandType)
        {
            OpCode = opCode;
            OpType = opType;
            OperandType = operandType;
        }
    }
}

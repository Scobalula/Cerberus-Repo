using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// Script Operation Types
    /// </summary>
    public enum ScriptOpType
    {
        None,
        StackPush,
        StackPop,
        Endon,
        Notification,
        Waittill,
        Call,
        JumpExpression,
        JumpCondition,
        Jump,
        SetVariable,
        Variable,
        VariableReference,
        Array,
        ArrayReference,
        ClearVariable,
        Object,
        ObjectReference,
        Cast,
        SingleToken,
        SingleOperand,
        DoubleOperand,
        Comparison,
        SizeOf,
        Switch,
        SwitchCases,
        Return,
    }
}

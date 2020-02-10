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
    public enum ScriptOperandType : byte
    {
        None,
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Float,
        Vector,
        VectorFlags,
        String,
        Call,
        FunctionPointer,
        Hash,
        VariableList,
        VariableName,
        VariableIndex,
        SwitchEnd,
    }
}

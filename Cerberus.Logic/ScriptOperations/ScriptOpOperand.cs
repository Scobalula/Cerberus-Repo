using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhilLibX.Mathematics;

namespace Cerberus.Logic
{
    public class ScriptOpOperand
    {
        /// <summary>
        /// Gets or Sets the Value of this operand
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(string val)
        {
            Value = val;
        }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(uint val)
        {
            Value = val;
        }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(int val)
        {
            Value = val;
        }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(float val)
        {
            Value = val;
        }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(float x, float y, float z)
        {
            Value = new Vector3(x, y, z);
        }

        /// <summary>
        /// Creates a Script Operand with the given value
        /// </summary>
        public ScriptOpOperand(string caseValue, int byteCodeOffset)
        {
            Value = new ScriptOpSwitch()
            {
                CaseValue = caseValue,
                ByteCodeOffset = byteCodeOffset
            };
        }

        public ScriptOpOperand(object value)
        {
            Value = value;
        }
    }
}

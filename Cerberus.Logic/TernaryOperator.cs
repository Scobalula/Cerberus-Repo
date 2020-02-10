using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    internal class TernaryOperator
    {
        public string Comparison { get; set; }
        public string OnTrue { get; set; }
        public string OnFalse { get; set; }

        public TernaryOperator(string comparison, string onTrue, string onFalse)
        {
            Comparison = comparison;
            OnTrue     = onTrue;
            OnFalse    = onFalse;
        }
    }
}

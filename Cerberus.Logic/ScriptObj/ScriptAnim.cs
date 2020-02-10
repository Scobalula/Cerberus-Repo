using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    public class ScriptAnim
    {
        /// <summary>
        /// Gets or Sets the Anim Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the offset to the Anim Name
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or Sets the pointer to where this anim is referenced
        /// </summary>
        public int Reference { get; set; }
    }
}

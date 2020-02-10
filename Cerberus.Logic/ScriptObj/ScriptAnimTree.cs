using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a Script Anim Tree
    /// </summary>
    public class ScriptAnimTree
    {
        /// <summary>
        /// Gets or Sets the Anim Tree Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the offset to the Anim Tree Name
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or Sets the list of references within the byte code
        /// </summary>
        public List<int> References { get; set; }

        /// <summary>
        /// Gets or Sets the list of animation references
        /// </summary>
        public List<ScriptAnim> AnimationReferences { get; set; }
    }
}

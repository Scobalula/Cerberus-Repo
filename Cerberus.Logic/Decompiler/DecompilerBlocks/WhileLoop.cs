// ------------------------------------------------------------------------
// Cerberus - A Call of Duty: Black Ops II/III GSC/CSC Decompiler
// Copyright (C) 2019 Philip/Scobalula
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

namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a While Loop
    /// </summary>
    internal class WhileLoop : DecompilerBlock
    {
        /// <summary>
        /// The comparison being made
        /// </summary>
        public string Comparison { get; set; }

        /// <summary>
        /// Initializes a While Loop
        /// </summary>
        /// <param name="startOffset"></param>
        /// <param name="endOffset"></param>
        public WhileLoop(int startOffset, int endOffset) : base(startOffset, endOffset) { }

        /// <summary>
        /// Gets the Header
        /// </summary>
        public override string GetHeader()
        {
            return string.Format("while({0})", Comparison);
        }

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

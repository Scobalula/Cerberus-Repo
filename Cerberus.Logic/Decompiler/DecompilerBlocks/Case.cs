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

namespace Cerberus.Logic
{
    internal class CaseBlock : DecompilerBlock
    {
        public string Value { get; set; }
        public int OriginalIndex { get; set; }
        public CaseBlock(int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            CanSkipZeroSize = true;
        }

        /// <summary>
        /// Gets the Header
        /// </summary>
        public override string GetHeader() => Value == "default" ? Value : string.Format("case {0}:", Value);

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => null;
    }
}

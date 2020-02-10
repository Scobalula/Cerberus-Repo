
namespace Cerberus.Logic
{
    /// <summary>
    /// A class to hold a Dev Block
    /// </summary>
    internal class DevBlock : DecompilerBlock
    {
        public DevBlock(int startOffset, int endOffset) : base(startOffset, endOffset)
        {
            RequiresBraces = false;
        }

        /// <summary>
        /// Gets the header
        /// </summary>
        public override string GetHeader() => "/#";

        /// <summary>
        /// Gets the footer
        /// </summary>
        public override string GetFooter() => "#/";
    }
}

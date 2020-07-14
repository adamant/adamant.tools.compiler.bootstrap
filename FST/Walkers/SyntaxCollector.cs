using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.FST.Walkers
{
    internal class SyntaxCollector : SyntaxWalker
    {
        private readonly List<ISyntax> syntax = new List<ISyntax>();

        public FixedList<ISyntax> Syntax => syntax.ToFixedList();

        protected override void WalkNonNull(ISyntax syntax)
        {
            this.syntax.Add(syntax);
        }
    }
}

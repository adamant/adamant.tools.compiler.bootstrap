using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class GotoStatement : BlockTerminatorStatement
    {
        public readonly int GotoBlockNumber;

        public GotoStatement(int gotoBlockNumber, TextSpan span, Scope scope)
            : base(span, scope)
        {
            GotoBlockNumber = gotoBlockNumber;
        }

        public override IEnumerable<int> OutBlocks()
        {
            yield return GotoBlockNumber;
        }

        public override Statement Clone()
        {
            return new GotoStatement(GotoBlockNumber, Span, Scope);
        }

        public override string ToString()
        {
            return $"goto bb{GotoBlockNumber} // at {Span} in {Scope}";
        }
    }
}

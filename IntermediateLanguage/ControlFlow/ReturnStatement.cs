using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ReturnStatement : BlockTerminatorStatement
    {
        public ReturnStatement(TextSpan span, Scope scope)
            : base(span, scope)
        {
        }

        public override IEnumerable<int> OutBlocks()
        {
            return Enumerable.Empty<int>();
        }

        public override Statement Clone()
        {
            return new ReturnStatement(Span, Scope);
        }

        public override string ToString()
        {
            return $"return // at {Span} in {Scope}";
        }
    }
}

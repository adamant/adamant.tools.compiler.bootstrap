using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfParameterNameSyntax : Syntax, ISelfParameterNameSyntax
    {
        public Promise<SelfParameterSymbol?> ReferencedSymbol { get; } = new Promise<SelfParameterSymbol?>();
        IPromise<BindingSymbol?> IParameterNameSyntax.ReferencedSymbol => ReferencedSymbol;

        public SelfParameterNameSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "self";
        }
    }
}

using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfParameterSyntax : ParameterSyntax, ISelfParameterSyntax
    {
        public override bool IsMutableBinding => MutableSelf;
        public bool MutableSelf { get; }
        public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
        public override IPromise<DataType> DataType { get; }

        public SelfParameterSyntax(TextSpan span, MaybeQualifiedName fullName, bool mutableSelf)
            : base(span, fullName, null)
        {
            MutableSelf = mutableSelf;
            DataType = Symbol.Select(s => s.DataType);
        }

        public override string ToString()
        {
            var value = "self";
            if (MutableSelf)
                value = "mut " + value;
            return value;
        }
    }
}

using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfParameterSyntax : ParameterSyntax, ISelfParameterSyntax
    {
        public bool IsMutableBinding => false;
        public bool MutableSelf { get; }
        public Promise<SelfParameterSymbol> Symbol { get; } = new Promise<SelfParameterSymbol>();
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        public override IPromise<DataType> DataType { get; }
        public SelfParameterSyntax(TextSpan span, bool mutableSelf)
            : base(span, null)
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

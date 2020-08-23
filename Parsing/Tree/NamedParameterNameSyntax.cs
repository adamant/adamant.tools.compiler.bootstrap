using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedParameterNameSyntax : Syntax, INamedParameterNameSyntax
    {
        public Name? Name { get; }
        public Promise<VariableSymbol?> ReferencedSymbol { get; } = new Promise<VariableSymbol?>();
        IPromise<BindingSymbol?> IParameterNameSyntax.ReferencedSymbol => ReferencedSymbol;

        public NamedParameterNameSyntax(TextSpan span, Name? name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name?.ToString() ?? "⧼unknown⧽";
        }
    }
}

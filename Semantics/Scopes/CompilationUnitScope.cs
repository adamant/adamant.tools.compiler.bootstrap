using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class CompilationUnitScope : LexicalScope
    {
        [NotNull] private static readonly FixedDictionary<string, ISymbol> Declarations = GetPrimitiveSymbols();

        [NotNull] public new CompilationUnitSyntax Syntax { get; }

        public CompilationUnitScope([NotNull] CompilationUnitSyntax compilationUnit)
            : base(compilationUnit)
        {
            Syntax = compilationUnit;
        }

        private static FixedDictionary<string, ISymbol> GetPrimitiveSymbols()
        {
            return PrimitiveSymbols.Instance
                .ToDictionary(s =>
                {
                    var name = (SimpleName)s.NotNull().Name;
                    Requires.That(nameof(name), name.IsSpecial);
                    return name.Text;
                }, s => s)
                .ToFixedDictionary();
        }

        [CanBeNull]
        public override ISymbol LookupGlobal([NotNull] Name name)
        {
            switch (name)
            {
                case SimpleName simpleName:
                    if (simpleName.IsSpecial)
                        return Declarations.TryGetValue(simpleName.Text, out var symbol)
                            ? symbol
                            : null;
                    return Lookup(simpleName.Text);
                case QualifiedName qualifiedName:
                    var qualifier = LookupGlobal(qualifiedName.Qualifier);
                    return qualifier?.Lookup(qualifiedName.UnqualifiedName.Text);
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }
    }
}

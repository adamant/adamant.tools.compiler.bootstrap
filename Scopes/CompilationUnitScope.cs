using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
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

        [NotNull]
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
                    return Lookup(simpleName);
                case QualifiedName qualifiedName:
                    var qualifier = LookupGlobal(qualifiedName.Qualifier);
                    return qualifier?.Lookup(qualifiedName.UnqualifiedName);
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }
    }
}

using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class LexicalScopeAttribute : SemanticAttribute
    {
        public const string Key = "LexicalScope";
        public override string AttributeKey => Key;

        public LexicalScopeAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LexicalScope Get(SyntaxBranchNode syntax)
        {
            return Attributes.Get(syntax, Key, Compute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TNameScope Get<TNameScope>(SyntaxBranchNode syntax)
            where TNameScope : LexicalScope
        {
            return (TNameScope)Attributes.Get(syntax, Key, Compute);
        }

        private LexicalScope Compute(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case CompilationUnitSyntax compilationUnit:
                    var globalNamespaceSymbol = SyntaxSymbol.Package.Children.Single();
                    var globalScope = new LexicalScope(globalNamespaceSymbol);
                    return globalScope;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}

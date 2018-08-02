using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class NodeAttribute : SemanticAttribute
    {
        public const string Key = "Node";
        public override string AttributeKey => Key;

        public NodeAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public Package this[PackageSyntax s] => Get<Package>(s);
        public CompilationUnit this[CompilationUnitSyntax s] => Get<CompilationUnit>(s);
        public Declaration this[DeclarationSyntax s] => Get<Declaration>(s);
        public AccessLevel this[]

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SemanticNode Get(SyntaxBranchNode syntax)
        {
            return Attributes.GetOrAdd(syntax, Key, Compute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TNode Get<TNode>(SyntaxBranchNode syntax)
            where TNode : SemanticNode
        {
            return (TNode)Attributes.GetOrAdd(syntax, Key, Compute);
        }

        private SemanticNode Compute(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case PackageSyntax package:
                    // To avoid potential contention, we build the syntax symbols
                    // before trying to create all the compilation unit nodes
                    // in parallel.
                    var syntaxSymbol = SyntaxSymbol.Package;
                    var compilationUnits = package.CompilationUnits
#if RELEASE
                        .AsParallel()
#endif
                        .Select(cu => Node[cu])
                        .ToList();
                    FunctionDeclaration entryPoint = null;
                    return new Package(package, AllDiagnostics[package], compilationUnits, entryPoint);
                case CompilationUnitSyntax compilationUnit:
                    return new CompilationUnit(compilationUnit, compilationUnit.Declarations.Select(d => Node[d]), AllDiagnostics[compilationUnit]);
                case FunctionDeclarationSyntax functionDeclaration:
                    return new FunctionDeclaration(functionDeclaration, AllDiagnostics[functionDeclaration],);
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}

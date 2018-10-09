using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string DiagnosticsAttribute = "Diagnostics";
        private const string AllDiagnosticsAttribute = "AllDiagnostics";

        private readonly IReadOnlyCollection<Diagnostic> noDiagnostics = new ReadOnlyCollectionBuilder<Diagnostic>().ToReadOnlyCollection();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<Diagnostic> IncompleteDiagnostics(SyntaxNode syntax)
        {
            return attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag);
        }

        private static ConcurrentBag<Diagnostic> NewDiagnosticsBag(SyntaxNode syntax)
        {
            return new ConcurrentBag<Diagnostic>();
        }

        private void AddDiagnostic(SyntaxNode syntax, Diagnostic diagnostic)
        {
            attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag).Add(diagnostic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<Diagnostic> AllDiagnostics(SyntaxNode s) =>
            attributes.GetOrAdd(s, AllDiagnosticsAttribute, ComputeAllDiagnostics);

        private IReadOnlyCollection<Diagnostic> ComputeAllDiagnostics(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case ExpressionSyntax _:
                case FunctionDeclarationSyntax _:
                case ParameterSyntax _:
                case ClassDeclarationSyntax _:
                case EnumStructDeclarationSyntax _:
                    Type(syntax);
                    break;
                case CompilationUnitSyntax _:
                case CompilationUnitNamespaceSyntax _:
                case StatementSyntax _:
                case PackageSyntax _:
                case IncompleteDeclarationSyntax _:
                    // These node types don't require any analysis
                    break;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }

            IReadOnlyCollection<Diagnostic> diagnostics;
            if (attributes.HasAttribute(syntax, "Diagnostics")
                && (diagnostics = IncompleteDiagnostics(syntax)).Any())
                return diagnostics;

            return noDiagnostics;
        }
    }
}

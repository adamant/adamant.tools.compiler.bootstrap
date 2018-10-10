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
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string DiagnosticsAttribute = "Diagnostics";
        private const string AllDiagnosticsAttribute = "AllDiagnostics";

        [NotNull] private readonly IReadOnlyCollection<Diagnostic> noDiagnostics = new ReadOnlyCollectionBuilder<Diagnostic>().ToReadOnlyCollection();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<Diagnostic> IncompleteDiagnostics([NotNull] SyntaxNode syntax)
        {
            return attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag);
        }

        private static ConcurrentBag<Diagnostic> NewDiagnosticsBag([NotNull] SyntaxNode syntax)
        {
            return new ConcurrentBag<Diagnostic>();
        }

        private void AddDiagnostic([NotNull] SyntaxNode syntax, [NotNull] Diagnostic diagnostic)
        {
            attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag).Add(diagnostic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public IReadOnlyCollection<Diagnostic> AllDiagnostics([NotNull] SyntaxNode s) =>
            attributes.GetOrAdd(s, AllDiagnosticsAttribute, ComputeAllDiagnostics);

        [NotNull]
        [ItemNotNull]
        private IReadOnlyCollection<Diagnostic> ComputeAllDiagnostics([NotNull]SyntaxNode syntax)
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

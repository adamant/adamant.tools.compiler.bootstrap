using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string DiagnosticsAttribute = "Diagnostics";
        private const string AllDiagnosticsAttribute = "AllDiagnostics";

        private readonly IReadOnlyCollection<DiagnosticInfo> noDiagnostics = new ReadOnlyCollectionBuilder<DiagnosticInfo>().ToReadOnlyCollection();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<DiagnosticInfo> IncompleteDiagnostics(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag);
        }

        private static ConcurrentBag<DiagnosticInfo> NewDiagnosticsBag(SyntaxBranchNode syntax)
        {
            return new ConcurrentBag<DiagnosticInfo>();
        }

        private void AddDiagnostic(SyntaxBranchNode syntax, DiagnosticInfo diagnostic)
        {
            attributes.GetOrAdd(syntax, DiagnosticsAttribute, NewDiagnosticsBag).Add(diagnostic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<DiagnosticInfo> AllDiagnostics(SyntaxBranchNode s) =>
            attributes.GetOrAdd(s, AllDiagnosticsAttribute, ComputeAllDiagnostics);

        private IReadOnlyCollection<DiagnosticInfo> ComputeAllDiagnostics(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case ExpressionSyntax _:
                case FunctionDeclarationSyntax _:
                case ParameterSyntax _:
                    Type(syntax);
                    break;
                case CompilationUnitSyntax _:
                case StatementSyntax _:
                case PackageSyntax _:
                    // These node types don't require any analysis
                    break;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }

            IReadOnlyCollection<DiagnosticInfo> diagnostics;
            if (attributes.HasAttribute(syntax, "Diagnostics")
                && (diagnostics = IncompleteDiagnostics(syntax)).Any())
                return diagnostics;

            return noDiagnostics;
        }
    }
}

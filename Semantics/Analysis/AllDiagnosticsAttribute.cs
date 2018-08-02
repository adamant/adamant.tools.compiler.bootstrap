using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    /// To get all diagnostics, you need to first do any needed semantic analysis
    /// and then check if there are diagnostics on the node
    public class AllDiagnosticsAttribute : SemanticAttribute
    {
        public const string Key = "AllDiagnostics";
        public override string AttributeKey => Key;
        private readonly IReadOnlyCollection<DiagnosticInfo> noDiagnostics = new ReadOnlyCollectionBuilder<DiagnosticInfo>().ToReadOnlyCollection();

        public AllDiagnosticsAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public IReadOnlyCollection<DiagnosticInfo> this[SyntaxBranchNode s] => Get(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<DiagnosticInfo> Get(SyntaxBranchNode syntax)
        {
            return Attributes.GetOrAdd(syntax, Key, Compute);
        }

        private IReadOnlyCollection<DiagnosticInfo> Compute(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case ExpressionSyntax expression:
                    //Type[expression];
                    break;
                case CompilationUnitSyntax _:
                    // These node types don't require any analysis
                    break;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }

            if (Attributes.HasAttribute(syntax, Diagnostics.AttributeKey)
                && Diagnostics[syntax].Any())
                return Diagnostics[syntax];

            return noDiagnostics;
        }
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    /// Diagnostics attribute is used to collect the diagnostics from other
    /// analysis. As such, the diagnostics for a syntax node may not be complete
    /// if analysis isn't complete. To get all diagnostics for a syntax node,
    /// use the AllDiagnosticsAttribute.
    public class DiagnosticsAttribute : SemanticAttribute
    {
        public const string Key = "Diagnostics";
        public override string AttributeKey => Key;

        public DiagnosticsAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public IReadOnlyCollection<DiagnosticInfo> this[SyntaxBranchNode s] => Get(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyCollection<DiagnosticInfo> Get(SyntaxBranchNode syntax)
        {
            return Attributes.GetOrAdd(syntax, Key, Compute);
        }

        public void Add(SyntaxBranchNode syntax, DiagnosticInfo diagnostic)
        {
            Attributes.GetOrAdd(syntax, Key, Compute).Add(diagnostic);
        }

        private static ConcurrentBag<DiagnosticInfo> Compute(SyntaxBranchNode syntax)
        {
            return new ConcurrentBag<DiagnosticInfo>();
        }
    }
}

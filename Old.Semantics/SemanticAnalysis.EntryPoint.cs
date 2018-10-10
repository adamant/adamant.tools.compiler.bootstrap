using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string EntryPointAttribute = "EntryPoint";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public FunctionDeclaration EntryPoint([NotNull] PackageSyntax s) => attributes.GetOrAdd(s, EntryPointAttribute, ComputeEntryPoint);

        [CanBeNull]
        private FunctionDeclaration ComputeEntryPoint([NotNull] PackageSyntax package)
        {
            return Node(package.DescendantsAndSelf()
                .OfType<FunctionDeclarationSyntax>()
                .SingleOrDefault(f => f.Name?.Value == "main"));
        }
    }
}

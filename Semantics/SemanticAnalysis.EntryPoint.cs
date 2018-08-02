using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string EntryPointAttribute = "EntryPoint";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionDeclaration EntryPoint(PackageSyntax s) => attributes.GetOrAdd(s, EntryPointAttribute, ComputeEntryPoint);

        private FunctionDeclaration ComputeEntryPoint(PackageSyntax package)
        {
            var entryPoint = package.DescendantBranchesAndSelf()
                .OfType<FunctionDeclarationSyntax>()
                .SingleOrDefault(f => f.Name.Value == "main");
            return entryPoint != null ? Node(entryPoint) : null;
        }
    }
}

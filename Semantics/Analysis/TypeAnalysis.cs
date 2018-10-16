using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class TypeAnalysis : DeclarationAnalysis
    {
        [NotNull] public DeclarationSyntax Syntax { get; }
        [NotNull] public new TypeDeclaration Semantics { get; }

        public TypeAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] DeclarationSyntax syntax,
            [NotNull] TypeDeclaration semantics)
            : base(file, scope, semantics)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(semantics), semantics);
            Syntax = syntax;
            Semantics = semantics;
        }
    }
}

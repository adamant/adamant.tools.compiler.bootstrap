using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class AnalysisContext
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public LexicalScope Scope { get; }

        public AnalysisContext([NotNull] CodeFile file, [NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            File = file;
            Scope = scope;
        }

        [NotNull]
        public AnalysisContext InFunctionBody([NotNull] NamedFunctionDeclarationSyntax syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            return new AnalysisContext(File, new FunctionScope(Scope, syntax));
        }

        [NotNull]
        public AnalysisContext InLocalVariableScope([NotNull] ExpressionSyntax syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            return new AnalysisContext(File, new LocalVariableScope(Scope, syntax));
        }

        [NotNull]
        public AnalysisContext WithGenericParameters([NotNull] MemberDeclarationSyntax syntax)
        {
            return new AnalysisContext(File, new GenericsScope(Scope, syntax));
        }

        [NotNull]
        public AnalysisContext InNamespace(
            [NotNull] NamespaceDeclarationSyntax @namespace,
            [NotNull] Name name)
        {
            return new AnalysisContext(File, new NamespaceScope(Scope, @namespace, name));
        }

        [NotNull]
        public AnalysisContext WithUsingDirectives([NotNull] NamespaceDeclarationSyntax @namespace)
        {
            return new AnalysisContext(File, new UsingDirectivesScope(Scope, @namespace));
        }
    }
}

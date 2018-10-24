using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class ScopeBinder
    {
        [NotNull] private readonly Dictionary<MemberDeclarationSyntax, MemberDeclarationAnalysis> declarations;
        [NotNull] private readonly Dictionary<string, IDeclarationAnalysis> globalDeclarations;

        public ScopeBinder([NotNull][ItemNotNull] IEnumerable<MemberDeclarationAnalysis> declarationAnalyses)
        {
            this.declarations = declarationAnalyses.ToDictionary(a => a.Syntax, a => a);

            globalDeclarations = this.declarations.Values.AssertNotNull()
                .Where(IsGlobalDeclaration)
                .ToDictionary(d => d.Name.Name.Text, d => d as IDeclarationAnalysis);
        }

        private static bool IsGlobalDeclaration([NotNull] IDeclarationAnalysis declaration)
        {
            return !declaration.Name.Qualifier.Any();
        }

        public void Bind([NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(scope), scope);
            switch (scope)
            {
                case CompilationUnitScope cuScope:
                    cuScope.Bind(globalDeclarations);
                    break;
                case FunctionScope functionScope:
                    var function = (FunctionDeclarationAnalysis)declarations[functionScope.Syntax].AssertNotNull();
                    var variableDeclarations = new Dictionary<string, IDeclarationAnalysis>();
                    foreach (var parameter in function.Parameters)
                        variableDeclarations.Add(parameter.Name.Name.Text, parameter);

                    functionScope.Bind(variableDeclarations);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(scope);
            }

            foreach (var nestedScope in scope.NestedScopes)
                Bind(nestedScope);
        }
    }
}

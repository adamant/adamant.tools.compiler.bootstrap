using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    /// Note: Naming this `Syntax` would conflict with the `Syntax` namespace
    public abstract class SyntaxNode : ISyntaxNodeOrToken
    {
        public IEnumerable<SyntaxNode> DescendantsAndSelf()
        {
            var nodes = new Stack<SyntaxNode>();
            while (nodes.TryPop(out var node))
            {
                switch (node)
                {
                    case null:
                        // If one of the descendants was null, not present,
                        // just skip it.
                        continue;
                    case PackageSyntax package:
                        nodes.PushRange(package.CompilationUnits.Reverse());
                        break;
                    case CompilationUnitSyntax compilationUnit:
                        nodes.PushRange(compilationUnit.Declarations.Reverse());
                        nodes.PushRange(compilationUnit.UsingDirectives.Reverse());
                        nodes.Push(compilationUnit.Namespace);
                        break;
                    case ClassDeclarationSyntax classDeclaration:
                        nodes.PushRange(classDeclaration.Members.Reverse());
                        break;
                    case ParameterSyntax parameter:
                        nodes.Push(parameter.Type);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(node);
                }
                yield return node;
            }
        }
    }
}

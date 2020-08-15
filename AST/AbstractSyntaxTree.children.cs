using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Collections.Generic;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [GeneratedCode("AdamantCompilerCodeGen", null)]
    public static class IAbstractSyntaxExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<IAbstractSyntax> Children(this IAbstractSyntax node)
        {
            switch(node)
            {
                default:
                    throw ExhaustiveMatch.Failed(node);
                case IClassDeclaration n:
                    foreach(var child in n.Members)
                        yield return child;
                    yield break;
                case IFunctionDeclaration n:
                    foreach(var child in n.Parameters)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IAbstractMethodDeclaration n:
                    yield return n.SelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    yield break;
                case IConcreteMethodDeclaration n:
                    yield return n.SelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IConstructorDeclaration n:
                    yield return n.ImplicitSelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IFieldDeclaration n:
                    yield break;
                case IAssociatedFunctionDeclaration n:
                    foreach(var child in n.Parameters)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case INamedParameter n:
                    if(!(n.DefaultValue is null))
                        yield return n.DefaultValue;
                    yield break;
                case ISelfParameter n:
                    yield break;
                case IFieldParameter n:
                    if(!(n.DefaultValue is null))
                        yield return n.DefaultValue;
                    yield break;
                case IBody n:
                    yield break;
                case IExpression n:
                    yield break;
            }
        }
    }
}

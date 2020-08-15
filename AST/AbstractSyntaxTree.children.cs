using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
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
                    yield break;
                case IAbstractMethodDeclaration n:
                    yield break;
                case IConcreteMethodDeclaration n:
                    yield break;
                case IConstructorDeclaration n:
                    yield return n.ImplicitSelfParameter;
                    yield break;
                case IFieldDeclaration n:
                    yield break;
                case IAssociatedFunctionDeclaration n:
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

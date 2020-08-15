using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Collections.Generic;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [GeneratedCode("AdamantCompilerCodeGen", null)]
    public static class ISyntaxExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<ISyntax> Children(this ISyntax node)
        {
            switch(node)
            {
                default:
                    throw ExhaustiveMatch.Failed(node);
                case ICompilationUnitSyntax n:
                    foreach(var child in n.UsingDirectives)
                        yield return child;
                    foreach(var child in n.Declarations)
                        yield return child;
                    yield break;
                case IUsingDirectiveSyntax n:
                    yield break;
                case INamespaceDeclarationSyntax n:
                    foreach(var child in n.UsingDirectives)
                        yield return child;
                    foreach(var child in n.Declarations)
                        yield return child;
                    yield break;
                case IClassDeclarationSyntax n:
                    foreach(var child in n.Members)
                        yield return child;
                    yield break;
                case IFunctionDeclarationSyntax n:
                    foreach(var child in n.Parameters)
                        yield return child;
                    if(!(n.ReturnType is null))
                        yield return n.ReturnType;
                    foreach(var child in n.ReachabilityAnnotations)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IAbstractMethodDeclarationSyntax n:
                    yield return n.SelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    if(!(n.ReturnType is null))
                        yield return n.ReturnType;
                    foreach(var child in n.ReachabilityAnnotations)
                        yield return child;
                    yield break;
                case IConcreteMethodDeclarationSyntax n:
                    yield return n.SelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    if(!(n.ReturnType is null))
                        yield return n.ReturnType;
                    foreach(var child in n.ReachabilityAnnotations)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IConstructorDeclarationSyntax n:
                    yield return n.ImplicitSelfParameter;
                    foreach(var child in n.Parameters)
                        yield return child;
                    foreach(var child in n.ReachabilityAnnotations)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case IFieldDeclarationSyntax n:
                    yield return n.Type;
                    yield break;
                case IAssociatedFunctionDeclarationSyntax n:
                    foreach(var child in n.Parameters)
                        yield return child;
                    if(!(n.ReturnType is null))
                        yield return n.ReturnType;
                    foreach(var child in n.ReachabilityAnnotations)
                        yield return child;
                    yield return n.Body;
                    yield break;
                case INamedParameterSyntax n:
                    yield return n.Type;
                    if(!(n.DefaultValue is null))
                        yield return n.DefaultValue;
                    yield break;
                case ISelfParameterSyntax n:
                    yield break;
                case IFieldParameterSyntax n:
                    if(!(n.DefaultValue is null))
                        yield return n.DefaultValue;
                    yield break;
                case IReachableFromAnnotationSyntax n:
                    foreach(var child in n.ReachableFrom)
                        yield return child;
                    yield break;
                case ICanReachAnnotationSyntax n:
                    foreach(var child in n.CanReach)
                        yield return child;
                    yield break;
                case IInvocableNameSyntax n:
                    yield break;
                case IArgumentSyntax n:
                    yield break;
                case IBodySyntax n:
                    foreach(var child in n.Statements)
                        yield return child;
                    yield break;
                case ITypeNameSyntax n:
                    yield break;
                case IOptionalTypeSyntax n:
                    yield return n.Referent;
                    yield break;
                case ICapabilityTypeSyntax n:
                    yield return n.ReferentType;
                    yield break;
                case IResultStatementSyntax n:
                    yield break;
                case IVariableDeclarationStatementSyntax n:
                    if(!(n.Type is null))
                        yield return n.Type;
                    yield break;
                case IExpressionStatementSyntax n:
                    yield break;
                case IAssignmentExpressionSyntax n:
                    yield break;
                case IBinaryOperatorExpressionSyntax n:
                    yield break;
                case IBlockExpressionSyntax n:
                    foreach(var child in n.Statements)
                        yield return child;
                    yield break;
                case IBoolLiteralExpressionSyntax n:
                    yield break;
                case IBorrowExpressionSyntax n:
                    yield break;
                case IBreakExpressionSyntax n:
                    yield break;
                case IFieldAccessExpressionSyntax n:
                    yield return n.Field;
                    yield break;
                case IForeachExpressionSyntax n:
                    if(!(n.Type is null))
                        yield return n.Type;
                    yield return n.Block;
                    yield break;
                case IFunctionInvocationExpressionSyntax n:
                    yield return n.FunctionNameSyntax;
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case IIfExpressionSyntax n:
                    yield return n.ThenBlock;
                    if(!(n.ElseClause is null))
                        yield return n.ElseClause;
                    yield break;
                case IImplicitImmutabilityConversionExpressionSyntax n:
                    yield return n.Expression;
                    yield break;
                case IImplicitNoneConversionExpressionSyntax n:
                    yield return n.Expression;
                    yield break;
                case IImplicitNumericConversionExpressionSyntax n:
                    yield return n.Expression;
                    yield break;
                case IImplicitOptionalConversionExpressionSyntax n:
                    yield return n.Expression;
                    yield break;
                case IIntegerLiteralExpressionSyntax n:
                    yield break;
                case ILoopExpressionSyntax n:
                    yield return n.Block;
                    yield break;
                case IMethodInvocationExpressionSyntax n:
                    yield return n.MethodNameSyntax;
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case IMoveExpressionSyntax n:
                    yield break;
                case INameExpressionSyntax n:
                    yield break;
                case INewObjectExpressionSyntax n:
                    yield return n.Type;
                    if(!(n.ConstructorName is null))
                        yield return n.ConstructorName;
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case INextExpressionSyntax n:
                    yield break;
                case INoneLiteralExpressionSyntax n:
                    yield break;
                case IReturnExpressionSyntax n:
                    yield break;
                case ISelfExpressionSyntax n:
                    yield break;
                case IShareExpressionSyntax n:
                    yield break;
                case IStringLiteralExpressionSyntax n:
                    yield break;
                case IUnaryOperatorExpressionSyntax n:
                    yield break;
                case IUnsafeExpressionSyntax n:
                    yield break;
                case IWhileExpressionSyntax n:
                    yield return n.Block;
                    yield break;
            }
        }
    }
}

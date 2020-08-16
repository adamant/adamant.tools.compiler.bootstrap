using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
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
                    foreach(var child in n.Statements)
                        yield return child;
                    yield break;
                case IResultStatement n:
                    yield return n.Expression;
                    yield break;
                case IVariableDeclarationStatement n:
                    if(!(n.Initializer is null))
                        yield return n.Initializer;
                    yield break;
                case IExpressionStatement n:
                    yield return n.Expression;
                    yield break;
                case IBlockExpression n:
                    foreach(var child in n.Statements)
                        yield return child;
                    yield break;
                case INewObjectExpression n:
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case IUnsafeExpression n:
                    yield return n.Expression;
                    yield break;
                case IBoolLiteralExpression n:
                    yield break;
                case IIntegerLiteralExpression n:
                    yield break;
                case INoneLiteralExpression n:
                    yield break;
                case IStringLiteralExpression n:
                    yield break;
                case IAssignmentExpression n:
                    yield return n.LeftOperand;
                    yield return n.RightOperand;
                    yield break;
                case IBinaryOperatorExpression n:
                    yield return n.LeftOperand;
                    yield return n.RightOperand;
                    yield break;
                case IUnaryOperatorExpression n:
                    yield return n.Operand;
                    yield break;
                case IIfExpression n:
                    yield return n.Condition;
                    yield return n.ThenBlock;
                    if(!(n.ElseClause is null))
                        yield return n.ElseClause;
                    yield break;
                case ILoopExpression n:
                    yield return n.Block;
                    yield break;
                case IWhileExpression n:
                    yield return n.Condition;
                    yield return n.Block;
                    yield break;
                case IForeachExpression n:
                    yield return n.InExpression;
                    yield return n.Block;
                    yield break;
                case IBreakExpression n:
                    if(!(n.Value is null))
                        yield return n.Value;
                    yield break;
                case INextExpression n:
                    yield break;
                case IReturnExpression n:
                    if(!(n.Value is null))
                        yield return n.Value;
                    yield break;
                case IImplicitImmutabilityConversionExpression n:
                    yield return n.Expression;
                    yield break;
                case IImplicitNoneConversionExpression n:
                    yield return n.Expression;
                    yield break;
                case IImplicitNumericConversionExpression n:
                    yield return n.Expression;
                    yield break;
                case IImplicitOptionalConversionExpression n:
                    yield return n.Expression;
                    yield break;
                case IFunctionInvocationExpression n:
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case IMethodInvocationExpression n:
                    yield return n.Context;
                    foreach(var child in n.Arguments)
                        yield return child;
                    yield break;
                case INameExpression n:
                    yield break;
                case ISelfExpression n:
                    yield break;
                case IFieldAccessExpression n:
                    yield return n.Context;
                    yield break;
                case IBorrowExpression n:
                    yield return n.Referent;
                    yield break;
                case IMoveExpression n:
                    yield return n.Referent;
                    yield break;
                case IShareExpression n:
                    yield return n.Referent;
                    yield break;
            }
        }
    }
}

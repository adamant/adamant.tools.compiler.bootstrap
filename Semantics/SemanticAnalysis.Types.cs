using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string TypeAttribute = "Type";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(ParameterSyntax syntax) => Type((SyntaxBranchNode)syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(TypeSyntax syntax) => Type((SyntaxBranchNode)syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, TypeAttribute, ComputeType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TDataType Type<TDataType>(SyntaxBranchNode syntax)
            where TDataType : DataType
        {
            return (TDataType)attributes.GetOrAdd(syntax, TypeAttribute, ComputeType);
        }

        private DataType ComputeType(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    var parameterTypes = function.Parameters.Select(Type);
                    var returnType = Type(function.ReturnType);
                    return new FunctionType(parameterTypes, returnType);
                case ParameterSyntax parameter:
                    return Type(parameter.Type);
                case PrimitiveTypeSyntax primitiveType:
                    return PrimitiveType.New(primitiveType.Keyword.Kind);
                case BinaryOperatorExpressionSyntax binaryOperatorExpression:
                    var leftOperandType = Type(binaryOperatorExpression.LeftOperand);
                    var rightOperandType = Type(binaryOperatorExpression.RightOperand);
                    if (leftOperandType != rightOperandType
                        || (binaryOperatorExpression.Operator.Kind == TokenKind.Plus
                        && leftOperandType == PrimitiveType.Bool))
                    {
                        AddDiagnostic(binaryOperatorExpression, Error.OperatorCannotBeAppliedToOperandsOfType(binaryOperatorExpression.Operator.Kind, leftOperandType, rightOperandType));
                        return DataType.Unknown;
                    }

                    return leftOperandType;
                case IdentifierNameSyntax identifierName:
                    var variableName = Name(identifierName);
                    var symbol = PackageSyntaxSymbol.Lookup(variableName);
                    var variableDeclaration = symbol.Declarations.Single();
                    return Type(variableDeclaration);
                case ReturnExpressionSyntax _:
                    return PrimitiveType.Never;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}

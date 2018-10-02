using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

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
        public ObjectType Type(ClassDeclarationSyntax syntax) => Type<ObjectType>(syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectType Type(EnumStructDeclarationSyntax syntax) => Type<ObjectType>(syntax);

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

                case ClassDeclarationSyntax classDeclaration:
                    return new ObjectType(Name(classDeclaration), false);

                case EnumStructDeclarationSyntax enumDeclaration:
                    return new ObjectType(Name(enumDeclaration), false);

                case IdentifierNameSyntax identifierName:
                    {
                        var name = Name(identifierName);
                        switch (name)
                        {
                            case VariableName variableName:
                                var variableSymbol = PackageSyntaxSymbol.Lookup(variableName);
                                var variableDeclaration = variableSymbol.Declaration;
                                return Type(variableDeclaration);

                            case ReferenceTypeName referenceTypeName:
                                var classSymbol = PackageSyntaxSymbol.Lookup(referenceTypeName);
                                var classDeclaration = classSymbol.Declaration;
                                return Type(classDeclaration);

                            case UnknownName _:
                                return DataType.Unknown;

                            default:
                                throw NonExhaustiveMatchException.For(name);
                        }
                    }

                case ReturnExpressionSyntax _:
                    return PrimitiveType.Never;

                case VariableDeclarationStatementSyntax variableDeclaration:
                    return Type(variableDeclaration.Type);

                case NewObjectExpressionSyntax newObjectExpression:
                    return Type(newObjectExpression.Type);

                case LifetimeTypeSyntax lifetimeType:
                    Lifetime lifetime;
                    if (lifetimeType.Lifetime.Kind == TokenKind.OwnedKeyword)
                        lifetime = OwnedLifetime.Instance;
                    else
                        lifetime = new NamedLifetime(lifetimeType.Lifetime.Text);
                    return new LifetimeType(Type(lifetimeType.TypeName), lifetime);

                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}

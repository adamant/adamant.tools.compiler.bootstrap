using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string TypeAttribute = "Type";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(ParameterSyntax syntax) => Type((SyntaxNode)syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(TypeSyntax syntax) => Type((SyntaxNode)syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectType Type(ClassDeclarationSyntax syntax) => Type<ObjectType>(syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectType Type(EnumStructDeclarationSyntax syntax) => Type<ObjectType>(syntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType Type(SyntaxNode syntax)
        {
            return attributes.GetOrAdd(syntax, TypeAttribute, ComputeType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TDataType Type<TDataType>(SyntaxNode syntax)
            where TDataType : DataType
        {
            return (TDataType)attributes.GetOrAdd(syntax, TypeAttribute, ComputeType);
        }

        private DataType ComputeType(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    var parameterTypes = function.ParametersList.Nodes().Select(Type);
                    var returnType = Type(function.ReturnTypeExpression);
                    return new FunctionType(parameterTypes, returnType);

                case ParameterSyntax parameter:
                    return Type(parameter.TypeExpression);

                case PrimitiveTypeSyntax primitiveType:
                    switch (primitiveType.Keyword)
                    {
                        case IntKeywordToken _:
                            return PrimitiveType.Int;
                        case UIntKeywordToken _:
                            return PrimitiveType.UInt;
                        case ByteKeywordToken _:
                            return PrimitiveType.Byte;
                        case SizeKeywordToken _:
                            return PrimitiveType.Size;
                        case VoidKeywordToken _:
                            return PrimitiveType.Void;
                        case BoolKeywordToken _:
                            return PrimitiveType.Bool;
                        case StringKeywordToken _:
                            return PrimitiveType.String;
                        default:
                            throw NonExhaustiveMatchException.For(primitiveType.Keyword);
                    }

                case BinaryOperatorExpressionSyntax binaryOperatorExpression:
                    var leftOperandType = Type(binaryOperatorExpression.LeftOperand);
                    var rightOperandType = Type(binaryOperatorExpression.RightOperand);
                    if (leftOperandType != rightOperandType
                        || (binaryOperatorExpression.Operator is PlusToken
                        && leftOperandType == PrimitiveType.Bool))
                    {
                        // TODO pass correct file and span
                        AddDiagnostic(binaryOperatorExpression, TypeError.OperatorCannotBeAppliedToOperandsOfType(null, new TextSpan(0, 0), binaryOperatorExpression.Operator, leftOperandType, rightOperandType));
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
                    return Type(variableDeclaration.TypeExpression);

                case NewObjectExpressionSyntax newObjectExpression:
                    return Type(newObjectExpression.Type);

                case LifetimeTypeSyntax lifetimeType:
                    Lifetime lifetime;
                    switch (lifetimeType.Lifetime)
                    {
                        case IdentifierToken identifier:
                            lifetime = new NamedLifetime(identifier.Value);
                            break;
                        case OwnedKeywordToken _:
                            lifetime = OwnedLifetime.Instance;
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(lifetimeType.Lifetime);
                    }
                    return new LifetimeType(Type(lifetimeType.TypeName), lifetime);

                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}

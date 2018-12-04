using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking
{
    public class TypeExpressionEvaluator
    {
        [NotNull]
        public static DataType EvaluateExpression(
            [NotNull] ExpressionSyntax typeExpression)
        {
            switch (typeExpression)
            {
                case IdentifierNameSyntax identifier:
                {
                    var identifierType = identifier.Type.Fulfilled();
                    switch (identifierType)
                    {
                        case Metatype metatype:
                            return metatype.Instance;
                        case TypeType _:
                            // It is a variable holding a type?
                            // for now, return a placeholder type
                            return DataType.Any;
                        case UnknownType _:
                            return DataType.Unknown;
                        default:
                            throw NonExhaustiveMatchException.For(identifierType);
                    }
                }
                case LifetimeTypeSyntax lifetimeType:
                {
                    var type = EvaluateExpression(lifetimeType.ReferentTypeExpression);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetimeToken = lifetimeType.Lifetime;
                    Lifetime lifetime;
                    switch (lifetimeToken)
                    {
                        case IOwnedKeywordToken _:
                            lifetime = OwnedLifetime.Instance;
                            break;
                        case IRefKeywordToken _:
                            lifetime = RefLifetime.Instance;
                            break;
                        case IIdentifierToken identifier:
                            lifetime = new NamedLifetime(identifier.Value);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(lifetimeToken);
                    }
                    if (type is ObjectType objectType)
                        return new LifetimeType(objectType, lifetime);
                    return DataType.Unknown;
                }
                case RefTypeSyntax refType:
                {
                    var referent = EvaluateExpression(refType.ReferencedType);
                    if (referent is ObjectType objectType)
                        return new RefType(objectType);
                    return DataType.Unknown;
                }
                case UnaryExpressionSyntax unaryOperatorExpression:
                    switch (unaryOperatorExpression.Operator)
                    {
                        case UnaryOperator.At:
                            if (unaryOperatorExpression.Operand.Type.Fulfilled() is Metatype metatype)
                                return new PointerType(metatype.Instance);
                            // TODO evaluate to type
                            return DataType.Unknown;
                        default:
                            // TODO evaluate to type
                            return DataType.Unknown;
                    }
                case GenericsInvocationSyntax _:
                case GenericNameSyntax _:
                {
                    var type = typeExpression.Type.Fulfilled();
                    if (type is Metatype metatype)
                        return metatype.Instance;

                    // TODO evaluate to type
                    return DataType.Unknown;
                }
                case BinaryExpressionSyntax _:
                    // TODO evaluate to type
                    return DataType.Unknown;
                case MutableTypeSyntax mutableType:
                    return EvaluateExpression(mutableType.ReferencedTypeExpression); // TODO make the type mutable
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}

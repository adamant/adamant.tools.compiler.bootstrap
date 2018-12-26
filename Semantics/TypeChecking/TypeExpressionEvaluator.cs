using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking
{
    public class TypeExpressionEvaluator
    {
        public static DataType EvaluateExpression(ExpressionSyntax typeExpression)
        {
            switch (typeExpression)
            {
                case IdentifierNameSyntax identifier:
                {
                    var identifierType = identifier.Type;
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
                case LifetimeNameSyntax lifetimeType:
                {
                    var type = EvaluateExpression(lifetimeType.ReferentTypeExpression);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetime = EvaluateLifetime(lifetimeType.Lifetime);
                    if (type is ObjectType objectType)
                        return new LifetimeType(objectType, lifetime);
                    return DataType.Unknown;
                }
                case LifetimeRelationSyntax lifetimeRelation:
                {
                    var type = EvaluateExpression(lifetimeRelation.ReferentTypeExpression);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetime = EvaluateLifetime(lifetimeRelation.Lifetime);
                    if (type is ObjectType objectType)
                        return new LifetimeBoundType(objectType, lifetimeRelation.Operator, lifetime);
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
                            if (unaryOperatorExpression.Operand.Type is Metatype metatype)
                                return new PointerType(metatype.Instance);
                            // TODO evaluate to type
                            return DataType.Unknown;
                        default:
                            // TODO evaluate to type
                            return DataType.Unknown;
                    }
                case GenericNameSyntax _:
                {
                    var type = typeExpression.Type;
                    if (type is Metatype metatype)
                        return metatype.Instance;

                    // TODO evaluate to type
                    return DataType.Unknown;
                }
                case BinaryExpressionSyntax _:
                    // TODO evaluate to type
                    return DataType.Unknown;
                case MutableExpressionSyntax mutableType:
                    return EvaluateExpression(mutableType.Expression); // TODO make the type mutable
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }

        public static Lifetime EvaluateLifetime(ILifetimeNameToken lifetimeToken)
        {
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

            return lifetime;
        }
    }
}

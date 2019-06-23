using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ReferenceType = Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceType;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    /// <summary>
    /// Type expressions are importantly different from regular expressions. In
    /// a standard expression, the type of the expression is what is being inferred.
    /// For type expressions, what is being done is figuring out what type the
    /// expression is naming. This is because the type of a type expression is
    /// always `Type` or some subtype of that. It is because of this last point
    /// that the methods in this class use the term "Check" because they are
    /// checking that the expression is of type `Type`.
    /// </summary>
    public class TypeExpressionEvaluator
    {
        public static DataType CheckExpression(ExpressionSyntax typeExpression)
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
                case ReferenceLifetimeSyntax referenceLifetime:
                {
                    var type = CheckExpression(referenceLifetime.ReferentTypeExpression);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetime = ResolveLifetime(referenceLifetime.Lifetime);
                    if (type is ReferenceType referenceType)
                        return referenceType.WithLifetime(lifetime);
                    return DataType.Unknown;
                }
                case RefTypeSyntax refType:
                {
                    var referent = CheckExpression(refType.ReferencedType);
                    if (referent is UserObjectType objectType)
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
                {
                    var type = CheckExpression(mutableType.Expression);
                    switch (type)
                    {
                        case UserObjectType objectType when objectType.DeclaredMutable:
                            return objectType.AsMutable();
                        default:
                            return DataType.Unknown;
                    }
                }
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }

        /// <summary>
        /// Since we aren't checking the type of lifetimes, we term this "Resolve"
        /// </summary>
        public static Lifetime ResolveLifetime(SimpleName lifetimeName)
        {
            if (lifetimeName.IsSpecial)
            {
                if (lifetimeName == SpecialName.Owned) return Lifetime.Owned;
                if (lifetimeName == SpecialName.Ref) return RefLifetime.Instance;
                if (lifetimeName == SpecialName.Forever) return Lifetime.Forever;
                throw NonExhaustiveMatchException.For(lifetimeName.Text);
            }

            return new NamedLifetime(lifetimeName.Text);
        }
    }
}

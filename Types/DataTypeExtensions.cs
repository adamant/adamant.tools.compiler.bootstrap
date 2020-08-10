using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public static class DataTypeExtensions
    {
        /// <summary>
        /// Returns the same type except with any mutability removed
        /// </summary>
        [DebuggerHidden]
        public static T ToReadOnly<T>(this T type)
            where T : DataType
        {
            return type.ToReadOnly_ReturnsSelf().Cast<T>();
        }

        /// <summary>
        /// Tests whether a place of the target type could be assigned a value of the source type.
        /// This does not account for implicit conversions, but does allow for borrowing
        /// and sharing. It also allows for isolated upgrading to mutable.
        /// </summary>
        public static bool IsAssignableFrom(this DataType target, DataType source)
        {
            switch (target, source)
            {
                case (_, _) when target.Equals(source):
                case (UnknownType _, _):
                case (_, UnknownType _):
                case (BoolType _, BoolConstantType _):
                    return true;
                case (AnyType targetReference, ReferenceType sourceReference):
                    return targetReference.ReferenceCapability.IsAssignableFrom(sourceReference.ReferenceCapability);
                case (ReferenceType _, AnyType _):
                    return false;
                case (ObjectType targetReference, ObjectType sourceReference):
                    return targetReference.ReferenceCapability.IsAssignableFrom(sourceReference.ReferenceCapability)
                           && targetReference.Name == sourceReference.Name
                           && targetReference.ContainingNamespace == sourceReference.ContainingNamespace;
                case (OptionalType targetOptional, OptionalType sourceOptional):
                    return IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Validates that a type as been assigned.
        /// </summary>
        [DebuggerHidden]
        public static DataType Assigned(this DataType? type)
        {
            return type ?? throw new InvalidOperationException("Type not assigned");
        }

        [DebuggerHidden]
        public static DataType Known(this DataType? type)
        {
            if (!type.Assigned().IsKnown) throw new InvalidOperationException($"Type {type} not known");

            return type!;
        }

        [DebuggerHidden]
        public static DataType Known(this Promise<DataType> promise)
        {
            var type = promise.Result;
            if (!type.IsKnown) throw new InvalidOperationException($"Type {type} not known");

            return type;
        }

        public static ReferenceType? UnderlyingReferenceType(this DataType type)
        {
            return type switch
            {
                ReferenceType referenceType => referenceType,
                OptionalType optionalType when optionalType.Referent is ReferenceType referenceType => referenceType,
                _ => null
            };
        }
    }
}

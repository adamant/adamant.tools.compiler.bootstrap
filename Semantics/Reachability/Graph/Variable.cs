using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A parameter, local variable, or field of `self`
    /// </summary>
    internal class Variable : StackPlace
    {
        public IBindingSymbol Symbol { get; }

        /// <summary>
        /// The type of this variable or field. If the original type was optional
        /// this is the underlying reference type.
        /// </summary>
        public ReferenceType Type { get; }

        private Variable(IBindingSymbol symbol)
        {
            Symbol = symbol;
            Type = symbol.Type.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        public static (CallerVariable?, Variable?) ForParameter(IParameterSyntax parameter)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = parameter.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return (null, null);

            CallerVariable? callerVariable = null;
            var parameterVariable = new Variable(parameter);

            var capability = referenceType.ReferenceCapability;
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case IsolatedMutable:
                case Isolated:
                {
                    // Isolated parameters are fully independent of the caller
                    var reference = Reference.ToNewParameterObject(parameter);
                    parameterVariable.AddReference(reference);
                }
                break;
                case Owned:
                case OwnedMutable:
                case Held:
                case HeldMutable:
                {
                    var reference = Reference.ToNewParameterObject(parameter);
                    parameterVariable.AddReference(reference);
                    var referencedObject = reference.Referent;

                    // Object to represent the bounding of the lifetime
                    callerVariable = CallerVariable.ForParameterWithObject(parameter);
                    referencedObject.ShareFrom(callerVariable);
                }
                break;
                case Borrowed:
                {
                    callerVariable = CallerVariable.ForParameterWithObject(parameter);
                    parameterVariable.BorrowFrom(callerVariable);
                }
                break;
                case Shared:
                {
                    callerVariable = CallerVariable.ForParameterWithObject(parameter);
                    parameterVariable.ShareFrom(callerVariable);
                }
                break;
                case Identity:
                {
                    callerVariable = CallerVariable.ForParameterWithObject(parameter);
                    parameterVariable.IdentityFrom(callerVariable);
                }
                break;
            }

            return (callerVariable, parameterVariable);
        }

        public static Variable? ForField(IFieldDeclarationSyntax field)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = field.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = new Variable(field);
            variable.AddReference(Reference.ToNewFieldObject(field));

            return variable;
        }

        public static Variable? Declared(IBindingSymbol symbol)
        {
            var referenceType = symbol.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new Variable(symbol);
        }

        public void Assign(TempValue temp)
        {
            AddReferences(temp.StealReferences());
        }
    }
}

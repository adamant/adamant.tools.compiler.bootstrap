using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class AssignablePlace : Place
    {
        public IBindingSymbol Symbol { get; }

        /// <summary>
        /// The type of this variable or field. If the original type was optional
        /// this is the underlying reference type.
        /// </summary>
        public ReferenceType Type { get; }

        protected AssignablePlace(IBindingSymbol symbol)
        {
            Symbol = symbol;
            Type = symbol.Type.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        public void Assign(ObjectPlace @object)
        {
            ClearReferences();
            switch (Type.ReferenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(Type.ReferenceCapability);
                case IsolatedMutable:
                case OwnedMutable:
                    Owns(@object, true);
                    break;
                case Isolated:
                case Owned:
                    Owns(@object, false);
                    break;
                case HeldMutable:
                    PotentiallyOwns(@object, true);
                    break;
                case Held:
                    PotentiallyOwns(@object, false);
                    break;
                case Borrowed:
                    Borrows(@object);
                    break;
                case Shared:
                    Shares(@object);
                    break;
                case Identity:
                    Identifies(@object);
                    break;
            }
        }
    }
}

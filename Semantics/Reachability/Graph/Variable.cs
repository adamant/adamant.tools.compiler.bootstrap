using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A parameter, local variable, or field of `self`
    /// </summary>
    internal class Variable : Place
    {
        public IBindingSymbol Symbol { get; }

        /// <summary>
        /// The type of this variable or field. If the original type was optional
        /// this is the underlying reference type.
        /// </summary>
        public ReferenceType Type { get; }

        public Variable(IBindingSymbol symbol)
        {
            Symbol = symbol;
            Type = symbol.Type.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        public void Assign(TempValue temp)
        {
            _ = temp;
            //ClearReferences();
            //switch (Type.ReferenceCapability)
            //{
            //    default:
            //        throw ExhaustiveMatch.Failed(Type.ReferenceCapability);
            //    case IsolatedMutable:
            //    case OwnedMutable:
            //        Owns(@object, true);
            //        break;
            //    case Isolated:
            //    case Owned:
            //        Owns(@object, false);
            //        break;
            //    case HeldMutable:
            //        PotentiallyOwns(@object, true);
            //        break;
            //    case Held:
            //        PotentiallyOwns(@object, false);
            //        break;
            //    case Borrowed:
            //        Borrows(@object);
            //        break;
            //    case Shared:
            //        Shares(@object);
            //        break;
            //    case Identity:
            //        Identifies(@object);
            //        break;
            //}
        }
    }
}

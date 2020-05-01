using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class AssignablePlace : Place
    {
        public ObjectPlace? Referent { get; private set; }

        protected AssignablePlace(PlaceIdentifier identifier)
            : base(identifier) { }

        public void Assign(ObjectPlace @object, DataType type)
        {
            Referent = null;
            if (type is ReferenceType referenceType)
            {
                switch (referenceType.ReferenceCapability)
                {
                    default:
                        throw new NotImplementedException(
                            $"Assignment not implemented for {referenceType.ReferenceCapability} references");
                    //    throw ExhaustiveMatch.Failed(referenceType.ReferenceCapability);
                    case Borrowed:
                        Borrows(@object);
                        break;
                    case Shared:
                        Shares(@object);
                        break;
                }
            }
        }
    }
}

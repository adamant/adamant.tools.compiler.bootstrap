using System;
using System.Linq;
using System.Reflection;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class VisitMethod
    {
        public MethodInfo Info { get; }
        public FixedList<Type> VisitTypes { get; }
        public VisitMethod(MethodInfo info)
        {
            Info = info;
            VisitTypes = info.GetCustomAttributes<VisitAttribute>()
                .Select(a => a.VisitType).ToFixedList();
            if (!VisitTypes.Any())
                VisitTypes = info.GetParameters()[0].ParameterType.Yield().ToFixedList();
        }

        public override int GetHashCode()
        {
            return Info.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as VisitMethod;
            return Info.Equals(other?.Info);
        }

        public bool Visits(Type type)
        {
            return VisitTypes.Any(t => t.IsAssignableFrom(type));
        }

        public VisitMethod MostSpecific(VisitMethod other)
        {
            var thisMoreSpecific = false;
            var otherMoreSpecific = false;
            foreach (var thisType in VisitTypes)
                foreach (var otherType in other.VisitTypes)
                {
                    thisMoreSpecific |= otherType.IsAssignableFrom(thisType);
                    otherMoreSpecific |= thisType.IsAssignableFrom(otherType);
                }

            // i.e. both true or both false
            if (thisMoreSpecific == otherMoreSpecific)
                throw new Exception("Neither method is more specific than the other");

            return thisMoreSpecific ? this : other;
        }
    }
}

using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class VisitAttribute : Attribute
    {
        public Type VisitType { get; }

        public VisitAttribute(Type visitType)
        {
            VisitType = visitType;
        }
    }
}

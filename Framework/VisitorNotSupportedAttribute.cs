using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VisitorNotSupportedAttribute : Attribute
    {
        public string Reason { get; }

        public VisitorNotSupportedAttribute(string reason = "")
        {
            Reason = reason;
        }
    }
}

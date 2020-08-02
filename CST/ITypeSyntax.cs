using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ITypeSyntax
    {
        [DisallowNull] DataType? NamedType { get; set; }
    }
}

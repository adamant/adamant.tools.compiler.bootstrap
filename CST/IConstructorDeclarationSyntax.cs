using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IConstructorDeclarationSyntax
    {
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<IConstructorParameterSyntax> Parameters { get; }
    }
}

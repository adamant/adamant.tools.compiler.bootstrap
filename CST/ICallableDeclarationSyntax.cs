using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    public partial interface ICallableDeclarationSyntax
    {
        new FixedList<IConstructorParameterSyntax> Parameters { get; }
    }
}

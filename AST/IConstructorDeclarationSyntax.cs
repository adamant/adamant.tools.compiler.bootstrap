using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<IStatementSyntax> Body { get; }
    }
}

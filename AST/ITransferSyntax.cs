using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// A transfer occurs when a value could be moved or borrowed. There are
    /// three places this happens:
    /// 1. Passing arguments
    /// 2. Returning a value
    /// 3. Assignment
    ///
    /// Calling a method is not a transfer context because while a transfer is happening,
    /// it is completely implicit whether it is moving, borrowing, borrowing mutably,
    /// or copying.
    /// </summary>
    [Closed(
        typeof(IMoveTransferSyntax),
        typeof(IMutableTransferSyntax),
        typeof(IImmutableTransferSyntax))]
    public interface ITransferSyntax : ISyntax
    {
        ref IExpressionSyntax Expression { get; }
        [DisallowNull] DataType? Type { get; set; }
    }
}

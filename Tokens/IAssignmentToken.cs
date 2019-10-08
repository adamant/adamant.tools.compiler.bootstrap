using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IEqualsToken),
        typeof(IPlusEqualsToken),
        typeof(IMinusEqualsToken),
        typeof(IAsteriskEqualsToken),
        typeof(ISlashEqualsToken))]
    public interface IAssignmentToken : IOperatorToken { }

    public partial interface IEqualsToken : IAssignmentToken { }
    public partial interface IPlusEqualsToken : IAssignmentToken { }
    public partial interface IMinusEqualsToken : IAssignmentToken { }
    public partial interface IAsteriskEqualsToken : IAssignmentToken { }
    public partial interface ISlashEqualsToken : IAssignmentToken { }
}

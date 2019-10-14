using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IStatementWalker
    {
        bool Enter(FixedList<IStatementSyntax> statements, ITreeWalker walker);
        void Exit(FixedList<IStatementSyntax> statements, ITreeWalker walker);

        bool Enter(IStatementSyntax statement, ITreeWalker walker);
        void Exit(IStatementSyntax statement, ITreeWalker walker);
    }
}

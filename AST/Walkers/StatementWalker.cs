using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class StatementWalker : IStatementWalker
    {
        public abstract bool Enter(FixedList<IStatementSyntax> statements, ITreeWalker walker);
        public virtual void Exit(FixedList<IStatementSyntax> statements, ITreeWalker walker) { }

        public abstract bool Enter(IStatementSyntax statement, ITreeWalker walker);
        public virtual void Exit(IStatementSyntax statement, ITreeWalker walker) { }
    }
}

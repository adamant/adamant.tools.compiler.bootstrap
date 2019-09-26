using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    /// <summary>
    /// This class stores inserted delete statements. This allows delete statements
    /// to be logically inserted into the control flow graph without actually
    /// modifying the control flow graph and throwing off associated live variables
    /// and borrow claims
    /// </summary>
    public class InsertedDeletes
    {
        private readonly Dictionary<Statement, List<DeleteStatement>> deletesAfter = new Dictionary<Statement, List<DeleteStatement>>();

        public IReadOnlyList<DeleteStatement> After(Statement statement)
        {
            return deletesAfter.TryGetValue(statement, out var deletes)
                ? (IReadOnlyList<DeleteStatement>)deletes.AsReadOnly()
                : FixedList<DeleteStatement>.Empty;
        }

        public void AddDeleteAfter(
            Statement statement,
            IPlace place,
            UserObjectType type,
            TextSpan span,
            Scope scope)
        {
            if (!deletesAfter.TryGetValue(statement, out var deletes))
            {
                deletes = new List<DeleteStatement>();
                deletesAfter.Add(statement, deletes);
            }
            deletes.Add(new DeleteStatement(place, type, span, scope));
        }
    }
}

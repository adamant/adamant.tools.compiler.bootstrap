namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public enum VariableReferenceKind
    {
        Assign = 0, // used for assigning to the variable
        Borrow, // exclusive access
        Share, // shared access
    }
}

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public interface IEmitter<in T>
    {
        void Emit(T value, Code code);
    }
}

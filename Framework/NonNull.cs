namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// The problem with this type is it doesn't correctly vary with T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public struct NonNull<T>
    //    where T : class
    //{
    //    public NonNull(T value)
    //    {
    //        Value = value ?? throw new ArgumentNullException(nameof(value));
    //    }

    //    [NotNull]
    //    public T Value { get; }

    //    public static implicit operator NonNull<T>([NotNull] T value)
    //    {
    //        return new NonNull<T>(value);
    //    }

    //    [NotNull]
    //    public static implicit operator T(NonNull<T> wrapper)
    //    {
    //        return wrapper.Value;
    //    }
    //}
}

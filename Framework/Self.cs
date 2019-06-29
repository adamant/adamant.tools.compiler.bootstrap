namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// This struct is used as a marker type for the returns `Self` type idiom.
    /// In this idiom, a `protected internal` method is declared on a class with
    /// return type `Self` and name ending in `ReturnsSelf`. For convenience,
    /// the base type may want to implement an implicit conversion to `Self`.
    /// A generic extension method is then used to re-expose the `ReturnsSelf`
    /// method as a method that actually has the same return type as the object
    /// it is called on. Note, it is up to implementors of the `ReturnsSelf`
    /// methods to actually return an object of the same type as the current
    /// class.
    /// </summary>
    public struct Self
    {
        private readonly object self;

        public Self(object self)
        {
            this.self = self;
        }

        public T Cast<T>()
        {
            return (T)self;
        }
    }
}

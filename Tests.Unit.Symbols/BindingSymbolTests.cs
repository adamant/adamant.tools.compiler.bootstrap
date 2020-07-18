using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class BindingSymbolTests
    {
        [Fact]
        public void Has_properties_constructed_with()
        {
            var name = Name.From("foo", "bar", "Baz");
            var type = new ObjectType(Name.From("Fake"), true, ReferenceCapability.Borrowed);

            var symbol = new BindingSymbol(name, true, type);

            Assert.Equal(name, symbol.FullName);
            Assert.True(symbol.IsMutableBinding);
            Assert.Equal(type, symbol.Type);
        }

        [Fact]
        public void Two_bindings_with_same_name_mutability_and_type_are_equal()
        {
            var binding1 = new BindingSymbol(Name.From("foo", "bar", "baz"), true,
                new ObjectType(Name.From("My_class"), true, ReferenceCapability.Borrowed));
            var binding2 = new BindingSymbol(Name.From("foo", "bar", "baz"), true,
                new ObjectType(Name.From("My_class"), true, ReferenceCapability.Borrowed));

            Assert.Equal(binding1, binding2);
        }

        [Fact]
        public void Bindings_with_different_mutability_are_not_equal()
        {
            var binding1 = new BindingSymbol(Name.From("foo", "bar", "baz"), true,
                new ObjectType(Name.From("My_class"), true, ReferenceCapability.Borrowed));
            var binding2 = new BindingSymbol(Name.From("foo", "bar", "baz"), false,
                new ObjectType(Name.From("My_class"), true, ReferenceCapability.Borrowed));

            Assert.NotEqual(binding1, binding2);
        }

        [Fact]
        public void Bindings_with_different_types_are_not_equal()
        {
            var binding1 = new BindingSymbol(Name.From("foo", "bar", "baz"), true,
                new ObjectType(Name.From("My_class1"), true, ReferenceCapability.Borrowed));
            var binding2 = new BindingSymbol(Name.From("foo", "bar", "baz"), true,
                new ObjectType(Name.From("My_class2"), true, ReferenceCapability.Borrowed));

            Assert.NotEqual(binding1, binding2);
        }
    }
}

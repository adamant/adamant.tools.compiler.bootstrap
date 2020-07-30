using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class MethodSymbolTests
    {
        [Fact]
        public void Methods_with_same_name_parameters_and_return_type_are_equal()
        {
            var selfParameter = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class"), false, ReferenceCapability.Borrowed));
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var method1 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Void, SymbolSet.Empty);
            var method2 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Void, SymbolSet.Empty);

            Assert.Equal(method1, method2);
        }

        [Fact]
        public void Methods_with_different_self_parameters_are_not_equal()
        {
            var selfParameter1 = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class1"), false, ReferenceCapability.Borrowed));
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var method1 = new MethodSymbol(Name.From("Fake"), selfParameter1, parameters, DataType.Void,
                SymbolSet.Empty);
            var selfParameter2 = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class2"), false, ReferenceCapability.Borrowed));
            var method2 = new MethodSymbol(Name.From("Fake"), selfParameter2, parameters, DataType.Void,
                SymbolSet.Empty);

            Assert.NotEqual(method1, method2);
        }

        [Fact]
        public void Methods_with_different_parameters_are_not_equal()
        {
            var selfParameter = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class"), false, ReferenceCapability.Borrowed));
            var parameters1 = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var method1 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters1, DataType.Void, SymbolSet.Empty);
            var parameters2 = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Int),
            }.ToFixedList();
            var method2 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters2, DataType.Void, SymbolSet.Empty);

            Assert.NotEqual(method1, method2);
        }

        [Fact]
        public void Methods_with_different_return_types_are_not_equal()
        {
            var selfParameter = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class"), false, ReferenceCapability.Borrowed));
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var method1 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Void, SymbolSet.Empty);
            var method2 = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Int, SymbolSet.Empty);

            Assert.NotEqual(method1, method2);
        }


        [Fact]
        public void Is_not_equal_to_equivalent_function()
        {
            // Note that methods should really have different names than functions,
            // but, just in case, we need to check method vs. function in equality.
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var selfParameter = new BindingSymbol(SpecialName.Self, true,
                new ObjectType(Name.From("My_Class"), false, ReferenceCapability.Borrowed));
            var method = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Void, SymbolSet.Empty);
            var func = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);

            // Note: assert false used to ensure which object Equals is called on
            Assert.False(method.Equals(func));
        }
    }
}

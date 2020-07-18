using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class FunctionSymbolTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(20)]
        public void Arity_is_number_of_parameters(int expectedParameters)
        {
            var parameters = Enumerable.Range(1, expectedParameters)
                                       .Select(n => new BindingSymbol(Name.From("param" + n), true, DataType.Int))
                                       .ToFixedList();
            var func = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);

            Assert.Equal(expectedParameters, func.Arity);
        }

        [Fact]
        public void Default_constructor_has_correct_properties()
        {
            // TODO It is a little strange that the type here has a reference capability
            var fullName = Name.From("My_Class");
            var type = new ObjectType(fullName, true, ReferenceCapability.Isolated);
            var defaultConstructor = FunctionSymbol.CreateDefaultConstructor(type);

            Assert.Equal(fullName.Qualify(SpecialName.New), defaultConstructor.FullName);
            Assert.Empty(defaultConstructor.Parameters);
            Assert.Equal(0, defaultConstructor.Arity);
            Assert.Equal(type, defaultConstructor.ReturnType);
            Assert.Empty(defaultConstructor.ChildSymbols);
        }

        [Fact]
        public void Functions_with_same_name_parameters_and_return_type_are_equal()
        {
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var func1 = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);
            var func2 = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);

            Assert.Equal(func1, func2);
        }

        [Fact]
        public void Functions_with_different_parameters_are_not_equal()
        {
            var parameters1 = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var func1 = new FunctionSymbol(Name.From("Fake"), parameters1, DataType.Void, SymbolSet.Empty);
            var parameters2 = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Int),
            }.ToFixedList();
            var func2 = new FunctionSymbol(Name.From("Fake"), parameters2, DataType.Void, SymbolSet.Empty);

            Assert.NotEqual(func1, func2);
        }

        [Fact]
        public void Functions_with_different_return_types_are_not_equal()
        {
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var func1 = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);
            var func2 = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Int, SymbolSet.Empty);

            Assert.NotEqual(func1, func2);
        }

        [Fact]
        public void Is_not_equal_to_equivalent_method()
        {
            // Note that methods should really have different names than functions,
            // but, just in case, we need to check method vs. function in equality.
            var parameters = new[]
            {
                new BindingSymbol(Name.From("a"), false, DataType.Int),
                new BindingSymbol(Name.From("b"), false, DataType.Bool),
            }.ToFixedList();
            var func = new FunctionSymbol(Name.From("Fake"), parameters, DataType.Void, SymbolSet.Empty);
            var selfParameter = new BindingSymbol(SpecialName.Self, true, new ObjectType(Name.From("My_Class"), false, ReferenceCapability.Borrowed));
            var method = new MethodSymbol(Name.From("Fake"), selfParameter, parameters, DataType.Void, SymbolSet.Empty);

            // Note: assert false used to ensure which object Equals is called on
            Assert.False(func.Equals(method));
        }
    }
}

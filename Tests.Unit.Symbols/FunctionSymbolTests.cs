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
    }
}

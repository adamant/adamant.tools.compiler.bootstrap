using System;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class OptionalTypeTests
    {
        [Fact]
        public void Optional_reference_has_reference_semantics()
        {
            var optionalAny = new OptionalType(new AnyType(ReferenceCapability.Isolated));

            Assert.Equal(TypeSemantics.Reference, optionalAny.Semantics);
        }

        [Fact]
        public void Optional_copy_type_has_copy_semantics()
        {
            var optionalBool = new OptionalType(DataType.Bool);

            Assert.Equal(TypeSemantics.Copy, optionalBool.Semantics);
        }

        /// <summary>
        /// The type `never?` has only one value, `none`. That value can be
        /// freely copied into any optional type, hence `never?` has copy
        /// semantics.
        /// </summary>
        [Fact]
        public void Optional_never_type_has_copy_semantics()
        {
            var optionalNever = new OptionalType(DataType.Never);

            Assert.Equal(TypeSemantics.Copy, optionalNever.Semantics);
        }

        /// <summary>
        /// The type `⧼unknown⧽?` is assignable into any optional type. Note though
        /// that it isn't assignable into non-optional types so it can't have
        /// never semantics. We assume it has the most lenient semantics possible
        /// for it, which is copy.
        /// </summary>
        [Fact]
        public void Optional_unknown_type_has_copy_semantics()
        {
            var optionalNever = new OptionalType(DataType.Unknown);

            Assert.Equal(TypeSemantics.Copy, optionalNever.Semantics);
        }

        [Fact(Skip = "There are no move types yet (and they can't be faked)")]
        public void Optional_move_type_has_move_semantics()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Cannot_have_optional_void_type()
        {
            Assert.Throws<ArgumentException>(() => new OptionalType(DataType.Void));
        }
    }
}

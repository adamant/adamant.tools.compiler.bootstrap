# Adamant Bootstrap Compiler

A bootstrap compiler for the [Adamant language](http://adamant-lang.org).  That is, a compiler for a subset of the language that will be used to write the Adamant compiler in Adamant. The compiler currently compiles a small subset of the Adamant language to C.

## Project Status: Alpha Active

The compiler is under active development. It is in a very early stage, and there are likely issues and limitations. APIs are subject to frequent breaking changes.

## Current Plan

The current plan is to slowly build up functionality by getting a series of katas and small programs working. These will likely be included in the conformance tests. After some very basic programs, various versions of the Fizz Buzz kata will be worked on. As this process is carried out, the plan will be to:

* Make the Adamant source code as correct as possible given all current ideas on the design of the language with the exception of specific features listed below.
* Ensure any language features used are in the language reference.
* If needed, parts of what will be the standard library can be created as compiler intrinsics at first, but they could be replaced with correct standard library code as quickly as possible.

### Excluded Language Features

The following features will not be implemented by this compiler even though they are described in the [language reference](https://github.com/adamant/adamant.language.reference/blob/master/src/book.md).

* Meta Functions: Functions which have generic parameters but not regular parameters and are evaluated only at compilet time.
* [Loop Labels](https://github.com/adamant/adamant.language.reference/blob/master/src/loop-expressions.md#loop-labels)
* [Document Comment Validation](https://github.com/adamant/adamant.language.reference/blob/master/src/documentation-comments.md#supported-markdown)

### Features Planned for After v1.0

A number of language features are planned for the future but not yet included in the language. For quick reference, SOME of those features are:

* Types
  * [128-bit Integer Types](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#128-bit-integer-types)
  * [128-bit Floating Point Type](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#128-bit-floating-point-type)
  * [Fixed Point Types](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#fixed-point-types)
  * [Decimal Types](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#decimal-types)
  * [Real Types](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#real-types)
* Using Directives
  * [Global and Package Qualified Using Directives](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#global-and-package-qualified-using-directives)
  * [Using Type Members](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#using-type-members)
* [Aliases](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#aliases)
* Expressions
  * [Multiline String Literals](https://github.com/adamant/adamant.language.reference/blob/master/src/planned-features.md#multiline-string-literals)

## Cleanup Tasks

None Currently

## Conventions

* Unit tests are in projects named `Tests.Unit.*`. This way, it is not inconsistent when further namespaces are nested inside them. If `Tests` were at the end of the name, then many namespaces would have it at the end, while nested ones would have it in the middle. This also allows conformance and integration tests to be grouped with them by placing them all under the `Tests` namespace.
* Reshaper nullability attributes are used to help find issues with null handling. Occasionally, the inference is incorrect because framework calls aren't correctly annotated. In such cases, the `AssertNotNull()` extension method is used. All methods taking non-null arguments should use `Requires.NotNull()` to enforce this. This is not necessary on values passed only to the base class constructor as the base constructor is responsible for checking. This null handling helps to support correct by construction and to flag errors as close to their source as possible.
* Namespace hierarchies are kept fairly flat. This is to avoid issues of needing too many `using` statements and moving types between them. A namespace should contain all classes that represent the same "kind" of entity without much regard to sub-kinds. Originally, this was not the case. Types were separated into many sub-namespaces, but this ended up being more trouble than it was worth. In practice, one still just used the go to type functionality to find types.

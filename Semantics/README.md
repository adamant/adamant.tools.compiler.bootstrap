# Adamant.Tools.Compiler.Bootstrap.Semantics

The semantic model is designed with a number of constraints in mind. While it is currently implemented in C# for a subset of the language, we are trying to plan for reimplementation in Adamant for the full language and support of incremental compilation. It is likely that a full design for these can't be anticipated, but we are still going to try to avoid designs that will be problematic later. The design tries to account for:

* Borrow checking and reference counting means that cycles need to be avoided. In particular, links across the tree are an issue. Consequently, links across the tree are stored as `Name` or `DataType` objects that can be used to look up the relevant nodes in the symbol tree.
* Incremental Compilation means that references to parent nodes are a problem because child nodes may be reused in the tree of a later compilation. Beyond this, no particular design for incremental compilation is being done. Note that while parent links can't be stored in the tree permanently, they are needed and useful on a temporary basis to allow things like lookup of names in lexical scopes.
* Arbitrary code execution at runtime will mean that there could be arbitrarily complex connections between which parts of the semantic tree must be evaluated before other parts. For example, the ability to declare the return type of a function to be the result of a meta-function and the ability of pure functions to be used in constant and generic argument contexts means that some functions will have to be fully analyzed and interpreted before other functions can even be typed.
* Parallel compilation will need to be supported to allow compilation to take full advantage of modern hardware. This means it must be possible to break the work into chunks while not producing undo contention.
* Strong typing is desired to avoid bugs involving missing values or values of incorrect types.

## Current Plan

To support the above design goals, the current plan is to analyze compilation units in parallel utilizing the `Lazy<T>` type to avoid duplicate work. Thus, if a task is analyzing a function and needs information about some other type, it will try to access the lazy property and if that value has not been evaluated yet, it will be evaluated. Then when the task for that compilation unit gets to it, it will already be evaluated and won't be computed again. The steps will thus be:

1. Build a tree of syntax symbols for all compilation units. Doing this synchronously eliminates the need for concurrency here, the tree becomes read-only after this point.
2. Build name scopes for each compilation unit in parallel. These are fully independent so no coordination is needed besides that they are all writing to the same annotation collection. By building them together, later steps do not have to worry about coordinating their construction. Note that because they are built from the top down but read from the bottom up, if we tried to build them as part of later steps, we would access some node but then need to search up the tree from the point to begin creating name scopes from.
3. Analyze each compilation unit it parallel. This allows the relatively independent creation of the semantic tree for each compilation unit. Partial types could be an issue with this that will have to be dealt with when we reach it.

Note that due to the complexity of the issues of dealing with cycles in evaluation, we are not currently attempting to handle this. Instead, we will rely on `Lazy<T>` to throw exceptions when cycles occur. This will not provide friendly error messages, but once the errors cycles can lead to are understood, we can develop a better strategy for handling them.

## Simplifications for Semantic Tree

* Don't include parenthesized expressions
* Unify the different kinds of blocks
* All loops as `loop`?

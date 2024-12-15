## What is this?

A small unit testing framework for Barotrauma "in memory" C# mods

Looks like this:
https://github.com/SomeRandomNoobKekeke/BaroUnitTest/blob/1ecfa4132d16730b6edbec0d92540fb17c9de480/CSharp/Shared/Test/SomeClassTest.cs#L40-L46

All the stuff is in "UnitTest.cs", i expect you to just copy-paste that code into your mods

You can do whatever you want with the code, i'll use it myself in a few of my mods, but i can't promise that i'll expand it into some full scale testing framework or make the real docs

There are some examples, it's all wrapped as Barotrauma mod, you can just put this into LocalMods and see how it works

## Why?

It's slightly more declarative than prints

## Basic Usage:

Create a class that inherits UnitTest

Methods ending in "Test" are considered test methods, add some

Add `Describe(string description, Action test)` blocks, those are creating test context and can be nested

Then add some `Expect(Action test)`, `Expect(Func<object> test)`, `Expect(object o)` those are creating test results

Then assess those results with `Expect(a).ToBeEqual(b)`, `Expect(a).ToBeNotEqual(b)`, `Expect(a).ToThrow()`, `Expect(a).ToNotThrow()`

Then you can run that test with UnitTest static methods: `Run(Type T, string method = null)`, `Run<RawType>(string method = null)`, `Run(string name, string method = null)`, `RunAll()`

## Advanced Usage:

You can also override `Prepare`, `Finalize`, `Execute` lifecycle methods for more control

You can see rough coverage estimate for some class with `Coverage(string typeName)`, `Coverage(Type original)`, it'll find matching class ending in "Test" and just match their methods with reflection

Classes and methods ending in "Test" are considered test classes and methods, if don't want them to end in "Test" you can add `[TestFor("something")]` attribute to override this

By default methods with special names and methods defined in base classes are excluded from coverage. If you want to exclude even more you can define `public static bool IsTestable(MemberInfo member, Type original)` on your test class that will be used instead of `UnitTest.IsTestable`

And tests should be in the same assembly as UnitTest or i'll never find them











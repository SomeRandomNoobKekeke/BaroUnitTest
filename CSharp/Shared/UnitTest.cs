using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace TemplateNamespace
{
  #region UnitTest

  public class TestForAttribute : System.Attribute
  {
    public string Name;
    public TestForAttribute(string name)
    {
      Name = name;
    }
  }

  public class UnitTest
  {
    public class TestContext
    {
      public string Description = "";

      public TestContext(string description = "")
      {
        Description = description;
      }

      public override string ToString() => Description;

      public static TestContext operator +(TestContext a, TestContext b)
          => new TestContext(a.Description + " | " + b.Description);
    }

    public class TestResult
    {
      public TestContext Context;
      public object Result;
      public bool? State;
      public bool Error;
      public Exception exception;

      public void ToBeEqual(object o) => State = Object.Equals(Result, o);
      public void ToBeNotEqual(object o) => State = !Object.Equals(Result, o);
      public void ToThrow() => State = Error;
      public void ToNotThrow() => State = !Error;
    }

    public static string GetTestClassName(string name) => name + "Test";
    public static string GetTestMethodName(string name) => name + "Test";
    public static bool IsTestable(MemberInfo member, Type original)
    {
      if (member is MethodInfo method)
      {
        if (
          method.IsSpecialName ||
          // method.IsConstructor ||
          method.DeclaringType != original
        ) return false;

        return true;
      }

      return false;
    }

    public static bool IsTestingMethod(MethodInfo mi) => mi.Name.EndsWith("Test") || Attribute.IsDefined(mi, typeof(TestForAttribute));

    public static Dictionary<string, Type> FindAllNonTests()
    {
      Assembly TestAssembly = Assembly.GetAssembly(typeof(UnitTest));

      Dictionary<string, Type> types = new();

      foreach (Type t in TestAssembly.GetTypes().Where(t => !t.IsSubclassOf(typeof(UnitTest))))
      {
        if (t.IsSpecialName) continue;
        if (t.Name.StartsWith("<>")) continue;
        if (t == typeof(UnitTest)) continue;
        if (t == typeof(TestForAttribute)) continue;
        if (t == typeof(TestContext)) continue;
        if (t == typeof(TestResult)) continue;
        if (t.IsAssignableTo(typeof(IAssemblyPlugin))) continue;

        types[t.Name] = t;
      }

      return types;
    }

    public static Dictionary<string, Type> FindAllTests()
    {
      Assembly TestAssembly = Assembly.GetAssembly(typeof(UnitTest));

      Dictionary<string, Type> testClasses = new();

      foreach (Type t in TestAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(UnitTest))))
      {
        TestForAttribute attribute = t.GetCustomAttribute<TestForAttribute>();
        if (attribute != null)
        {
          testClasses[GetTestClassName(attribute.Name)] = t;
        }
        else
        {
          testClasses[t.Name] = t;
        }
      }

      return testClasses;
    }

    public static Dictionary<string, MethodInfo> FindAllTestMethods(Type testType)
    {
      Dictionary<string, MethodInfo> testMethods = new();

      foreach (MethodInfo mi in testType.GetMethods(AccessTools.all))
      {
        if (IsTestingMethod(mi))
        {
          TestForAttribute attribute = mi.GetCustomAttribute<TestForAttribute>();
          if (attribute != null)
          {
            testMethods[GetTestMethodName(attribute.Name)] = mi;
          }
          else
          {
            testMethods[mi.Name] = mi;
          }
        }
      }

      return testMethods;
    }



    public static void Coverage(IEnumerable<Type> originalTypes)
    {
      Dictionary<string, Type> AllTests = FindAllTests();

      foreach (Type original in originalTypes)
      {
        if (original == null) continue;

        Type testType = AllTests.GetValueOrDefault(GetTestClassName(original.Name));
        if (testType == null)
        {
          Log($"No test class for type {original}");
        }
        else
        {
          Coverage(original, testType);
        }
      }
    }

    public static void Coverage(string typeName)
    {
      if (typeName == null || typeName == "") { Log($"for what?"); return; };

      List<Type> TestAssemblyTypes = Assembly.GetAssembly(typeof(UnitTest)).GetTypes().ToList();

      Type original = null;

      original ??= TestAssemblyTypes.Find(T => T.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase));
      original ??= TestAssemblyTypes.Find(T => T.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

      if (original != null)
      {
        Coverage(original);
        return;
      }

      List<Type> BaroAssemblyTypes = Assembly.GetAssembly(typeof(GameMain)).GetTypes().ToList();

      original ??= BaroAssemblyTypes.Find(T => T.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase));
      original ??= BaroAssemblyTypes.Find(T => T.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

      Coverage(original);
    }

    public static void Coverage(Type original)
    {
      if (original == null) { Log($"No such type"); return; };

      Dictionary<string, Type> AllTests = FindAllTests();

      Type testType = AllTests.GetValueOrDefault(GetTestClassName(original.Name));
      if (testType == null)
      {
        Log($"No test class for type {original}");
      }
      else
      {
        Coverage(original, testType);
      }
    }

    public static void Coverage(Type original, Type testType)
    {
      if (original == null) { Log($"original type is null"); return; }
      if (testType == null) { Log($"test type is null"); return; }

      MethodInfo customIsTestable = testType.GetMethod("IsTestable", AccessTools.all);
      Dictionary<string, MethodInfo> testMethods = FindAllTestMethods(testType);

      Log($"Coverage for {original.Name}:");
      try
      {
        foreach (MemberInfo mi in original.GetMembers(AccessTools.all))
        {
          bool testable = true;

          if (customIsTestable != null)
          {
            testable = Convert.ToBoolean(customIsTestable.Invoke(null, new object[] { mi, original }));
          }
          else
          {
            testable = IsTestable(mi, original);
          }

          if (!testable) continue;

          bool covered = testMethods.ContainsKey(GetTestMethodName(mi.Name));

          if (covered) Log($"  {mi}", Color.Lime);
          else Log($"  {mi}", Color.Gray);
        }
      }
      catch (Exception e) { Log(e, Color.Orange); }
    }



    public static void RunAll()
    {
      IEnumerable<Type> AllTests = FindAllTests().Values;
      if (AllTests.Count() == 0) Log($"no tests");
      foreach (Type T in AllTests) { Run(T); }
    }

    public static void Run(string name, string method = null)
    {
      Dictionary<string, Type> AllTests = FindAllTests();

      if (String.Equals("all", name, StringComparison.OrdinalIgnoreCase))
      {
        if (AllTests.Count == 0)
        {
          Log($"no tests");
        }
        else
        {
          foreach (Type T in AllTests.Values) { Run(T); }
        }

        return;
      }

      if (AllTests.ContainsKey(name))
      {
        Run(AllTests[name], method);
        return;
      }

      if (AllTests.ContainsKey(GetTestClassName(name)))
      {
        Run(AllTests[GetTestClassName(name)], method);
        return;
      }

      Log($"{name} not found");
    }

    public static void Run<RawType>(string method = null) => Run(typeof(RawType), method);
    public static void Run(Type T, string method = null)
    {
      if (!T.IsSubclassOf(typeof(UnitTest)))
      {
        Log($"{T} is not a test!");
        return;
      }


      Log($"------------------------");
      Log($"Running {T}");
      UnitTest test = (UnitTest)Activator.CreateInstance(T);

      try
      {
        test.Prepare();
        test.Execute(method);
      }
      catch (Exception e)
      {
        Log($"{T} Execution failed with:\n{e}", Color.Yellow);
      }
      finally
      {
        test.Finalize();
      }

      test.PrintResults();
    }

    public List<TestResult> Results = new List<TestResult>();
    public TestContext Context = new TestContext();
    public virtual void Prepare() { }

    public virtual void Finalize() { }

    public virtual void Execute(string method = null)
    {
      if (method == null)
      {
        IEnumerable<MethodInfo> methods = this.GetType().GetMethods().Where(mi => IsTestingMethod(mi));

        foreach (MethodInfo mi in methods)
        {
          Describe(mi.Name, () => mi.Invoke(this, new object[] { }));
        }
      }
      else
      {
        MethodInfo mi = this.GetType().GetMethod(method);
        mi ??= this.GetType().GetMethod(GetTestMethodName(method));

        if (mi != null)
        {
          Describe(mi.Name, () => mi.Invoke(this, new object[] { }));
        }
        else
        {
          Log($"method {method} in {this.GetType()} not found", Color.Orange);
        }
      }
    }


    public void Describe(string description, Action test)
    {
      TestContext oldContext = Context;
      Context = oldContext + new TestContext(description);
      test();
      Context = oldContext;
    }


    public TestResult Expect(Action test)
    {
      TestResult result = new TestResult();
      result.Context = Context;

      try
      {
        test();
      }
      catch (Exception e)
      {
        result.Error = true;
        result.exception = e;
      }

      Results.Add(result);
      return result;
    }
    public TestResult Expect(Func<object> test)
    {
      TestResult result = new TestResult();
      result.Context = Context;

      try
      {
        result.Result = test();
      }
      catch (Exception e)
      {
        result.Error = true;
        result.exception = e;
      }

      Results.Add(result);
      return result;
    }

    public TestResult Expect(object o)
    {
      TestResult result = new TestResult();
      result.Context = Context;

      result.Result = o;

      Results.Add(result);
      return result;
    }

    public void PrintResults()
    {
      int passed = 0;
      foreach (TestResult tr in Results)
      {
        if (tr.State.HasValue && tr.State.Value) passed++;

        Color cl;
        if (tr.State.HasValue)
        {
          cl = tr.State.Value ? Color.Lime : Color.Red;
        }
        else
        {
          cl = Color.White;
        }

        object result = tr.Error ? tr.exception.Message : tr.Result;

        Log($"{tr.Context} [{result}]", cl);
      }

      string conclusion = passed == Results.Count ? "All Passed" : "Failed";

      Log($"\n{passed}/{Results.Count} {conclusion}");
    }

    public UnitTest()
    {
      Context = new TestContext(this.GetType().Name);
    }

    public static void Log(object msg, Color? cl = null)
    {
      cl ??= Color.Cyan;
      LuaCsLogger.LogMessage($"{msg ?? "null"}", cl * 0.8f, cl);
    }
  }

  #endregion

}
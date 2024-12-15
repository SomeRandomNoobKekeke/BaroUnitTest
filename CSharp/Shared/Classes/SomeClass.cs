using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;


namespace TemplateNamespace
{
  public class SomeClass
  {
    public int SomeMethod()
    {
      return 2 + 2;
    }

    public void AnotherMethod()
    {
      throw new Exception("lul");
    }

    public static int Sum(int a, int b)
    {
      if (a == 0 && b == 0) return 0;
      if (a == 0 && b == 1) return 1;
      if (a == 0 && b == 2) return 2;
      if (a == 1 && b == 2) return 3;
      if (a == 2 && b == 2) return 4;
      return 0;
    }

    public void NotCovered()
    {

    }
  }
}
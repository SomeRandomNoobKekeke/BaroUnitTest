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
  public class SomeClassTest : UnitTest
  {
    public SomeClass o;

    public override void Prepare()
    {
      o = new SomeClass();
    }

    public override void Finalize()
    {
      o = null;
    }


    public void MegaTest()
    {
      Describe("lets run all at once", () =>
      {
        SomeMethodTest();
        Guh();
      });
    }


    public void SomeMethodTest()
    {
      Describe("2 + 2 should be == 4", () =>
      {
        Expect(o.SomeMethod()).ToBeEqual(4);
      });
    }

    [TestFor("AnotherMethod")]
    public void Guh()
    {
      Describe("should not throw anything funny", () =>
      {
        Expect(() => o.AnotherMethod()).ToNotThrow();
      });
    }

    public void SumTest()
    {
      for (int a = 0; a < 4; a++)
      {
        for (int b = 0; b < 4; b++)
        {
          Describe($"{a} + {b} =", () =>
          {
            Expect(SomeClass.Sum(a, b)).ToBeEqual(a + b);
          });
        }
      }
    }
  }
}
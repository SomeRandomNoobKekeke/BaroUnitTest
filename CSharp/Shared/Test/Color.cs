using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Barotrauma.Extensions;

namespace TemplateNamespace
{
  public class ColorTest : UnitTest
  {
    public void MultiplyTest()
    {
      Describe("new Color(8, 0, 0)", () =>
      {
        Color cl = new Color(8, 0, 0);
        Describe("*2", () => Expect(cl.Multiply(2)).ToBeEqual(new Color(16, 0, 0, 255)));
        Describe("*-1", () => Expect(cl.Multiply(-1)).ToBeEqual(new Color(0, 0, 0, 0)));
        Describe("*1.5", () => Expect(cl.Multiply(1.5f)).ToBeEqual(new Color(12, 0, 0, 255)));
        Describe("*0", () => Expect(cl.Multiply(0)).ToBeEqual(new Color(0, 0, 0, 0)));
        Describe("*1000", () => Expect(cl.Multiply(1000)).ToBeEqual(new Color(255, 0, 0, 255)));
      });
    }
  }
}
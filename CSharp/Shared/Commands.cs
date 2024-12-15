using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TemplateNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static List<DebugConsole.Command> AddedCommands = new List<DebugConsole.Command>();
    public static void AddCommands()
    {
      AddedCommands ??= new List<DebugConsole.Command>();


      AddedCommands.Add(new DebugConsole.Command("runtest", "", (string[] args) =>
      {
        if (args.Length == 0) { Mod.Log("mb name?"); return; }
        UnitTest.Run(args[0], args.ElementAtOrDefault(1));
      },
      () => new string[][] { UnitTest.FindAllTests().Values.Select(T => T.Name).Append("all").ToArray() }));

      AddedCommands.Add(new DebugConsole.Command("coverage", "", (string[] args) =>
      {
        UnitTest.Coverage(args.ElementAtOrDefault(0));
      }, () => new string[][] { UnitTest.FindAllNonTests().Values.Select(T => T.Name).ToArray() }));


      DebugConsole.Commands.InsertRange(0, AddedCommands);
    }

    public static void RemoveCommands()
    {
      AddedCommands.ForEach(c => DebugConsole.Commands.Remove(c));
      AddedCommands.Clear();
    }

    // public static void permitCommands(Identifier command, ref bool __result)
    // {
    //   if (AddedCommands.Any(c => c.Names.Contains(command.Value))) __result = true;
    // }
  }
}
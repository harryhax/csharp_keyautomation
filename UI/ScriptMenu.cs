using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Csharp_GTA_KeyAutomation.Automation.Engine;
using Csharp_GTA_KeyAutomation.Automation.Models;
using Csharp_GTA_KeyAutomation.Automation.Parsing;
using Csharp_GTA_KeyAutomation.ImageCapture;
using Csharp_GTA_KeyAutomation.Input;
using Csharp_GTA_KeyAutomation.Screen;

namespace Csharp_GTA_KeyAutomation.UI;

public static class ScriptMenu
{
    public static async Task ShowAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Scripts");
            Console.WriteLine("--------------------------------");

            var baseDir = AppContext.BaseDirectory;

            var approvedPath = Path.Combine(baseDir, "Scripts");
            var customPath = Path.Combine(baseDir, "Scripts_Custom");

            var approvedScripts = ScriptLoader.LoadScriptDescriptors(approvedPath);
            var customScripts = ScriptLoader.LoadScriptDescriptors(customPath);

            var menuItems = new List<ScriptMenuItem>();

            if (approvedScripts.Count > 0)
            {
                menuItems.Add(new ScriptMenuItem
                {
                    Type = ScriptMenuItemType.Header,
                    Title = "=== Public Scripts ===\n"
                });

                foreach (var s in approvedScripts)
                {
                    menuItems.Add(new ScriptMenuItem
                    {
                        Type = ScriptMenuItemType.Script,
                        Title = s.Title,
                        FilePath = s.FilePath,
                        IsCustom = false
                    });
                }

                Console.WriteLine();
            }

            if (customScripts.Count > 0)
            {
                menuItems.Add(new ScriptMenuItem
                {
                    Type = ScriptMenuItemType.Header,
                    Title = "\n=== Custom Scripts ===\n"
                });

                foreach (var s in customScripts)
                {
                    menuItems.Add(new ScriptMenuItem
                    {
                        Type = ScriptMenuItemType.Script,
                        Title = s.Title,
                        FilePath = s.FilePath,
                        IsCustom = true
                    });
                }
            }

            if (!menuItems.Any(m => m.Type == ScriptMenuItemType.Script))
            {
                Console.WriteLine("No scripts found.");
                Thread.Sleep(1500);
                return;
            }

            var indexMap = new Dictionary<int, ScriptMenuItem>();
            int displayIndex = 1;

            foreach (var item in menuItems)
            {
                if (item.Type == ScriptMenuItemType.Header)
                {
                    Console.WriteLine(item.Title);
                }
                else
                {
                    Console.WriteLine($"{displayIndex}) {item.Title}");
                    indexMap[displayIndex] = item;
                    displayIndex++;
                }
            }

            Console.WriteLine("\n--------------------------------");
            Console.WriteLine("b) Back");
            Console.WriteLine("q) Quit");
            Console.WriteLine("--------------------------------");

            Console.Write("\nSelect option: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            if (string.Equals(input, "b", StringComparison.OrdinalIgnoreCase))
                return;

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                Environment.Exit(0);

            if (!int.TryParse(input, out int selectedIndex) ||
                !indexMap.ContainsKey(selectedIndex))
            {
                Console.WriteLine("Invalid selection.");
                Thread.Sleep(1000);
                continue;
            }

            var selectedItem = indexMap[selectedIndex];
            await ShowScriptDetails(selectedItem);
        }
    }

    private static async Task ShowScriptDetails(ScriptMenuItem item)
    {
        var scriptResult = ScriptLoader.LoadFullScript(item.FilePath!);

        Console.Clear();
        Console.WriteLine(scriptResult.Title);
        Console.WriteLine(new string('-', scriptResult.Title.Length));
        Console.WriteLine();
        Console.WriteLine(scriptResult.Description);

        Console.WriteLine("\n--------------------------------");
        Console.WriteLine("Press enter to run or b to go back");
   
        var input = Console.ReadLine()?.Trim();

        if (string.Equals(input, "b", StringComparison.OrdinalIgnoreCase))
            return;

        if (!string.IsNullOrEmpty(input))
            return;

        if (scriptResult.Script == null)
        {
            Console.WriteLine("No Run section defined.");
            Thread.Sleep(1500);
            return;
        }

        IScreenCapture screen =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsScreenCapture()
                : new MacScreenCapture();

        IKeyboard keyboard =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsKeyboard()
                : new MacKeyboard();

        var runtime = new AutomationRuntime(screen, keyboard);
        var engine = new AutomationEngine(runtime);

        await engine.RunScriptAsync(scriptResult.Script);
    }
}

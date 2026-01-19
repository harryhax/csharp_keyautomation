using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Csharp_GTA_KeyAutomation.Automation.Engine;
using Csharp_GTA_KeyAutomation.Automation.Parsing;
using Csharp_GTA_KeyAutomation.ImageCapture;
using Csharp_GTA_KeyAutomation.Input;
using Csharp_GTA_KeyAutomation.Screen;
using Csharp_GTA_KeyAutomation.Automation.Scripts;


class Program
{
    static async Task Main()
    {
        ScriptRepositorySync.BaseUrl =
          "https://raw.githubusercontent.com/harryhax/harrykey_scripts/main/";

        ScriptRepositorySync.Debug = false;

        var baseDir = AppContext.BaseDirectory;

        Console.WriteLine("Syncing scripts...\n");

        try
        {
            await ScriptRepositorySync.SyncAsync(baseDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Script sync failed:");
            Console.WriteLine(ex.Message);
            Console.WriteLine("\nPress ENTER to continue anyway.");
            Console.ReadLine();
        }


        while (true)
        {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("---------");
            Console.WriteLine("1) Configuration");
            Console.WriteLine("2) Scripts");
            Console.WriteLine("Q) Quit");

            Console.Write("\nSelect option: ");
            var input = Console.ReadLine();

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                return;

            switch (input)
            {
                case "1":
                    ShowConfigurationMenu();
                    break;

                case "2":
                    await ShowScriptsMenu();
                    break;

                default:
                    Console.WriteLine("Invalid selection.");
                    break;
            }
        }
    }

    static void ShowConfigurationMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Configuration");
            Console.WriteLine("-------------");
            Console.WriteLine("1) Screen Capture every X seconds (DEBUG / DISK)");
            Console.WriteLine("2) Back to Main");

            Console.Write("\nSelect option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    RunScreenCaptureLoop();
                    break;

                case "2":
                    return;

                default:
                    Console.WriteLine("Invalid selection.");
                    Thread.Sleep(1000);
                    break;
            }
        }
    }

    /// <summary>
    /// DEBUG ONLY – writes frames to disk.
    /// Never used during automation.
    /// </summary>
    static async void RunScreenCaptureLoop()
    {
        Console.Clear();
        Console.Write("Enter capture interval in seconds: ");
        var input = Console.ReadLine();

        if (!int.TryParse(input, out int intervalSeconds) || intervalSeconds <= 0)
        {
            Console.WriteLine("Invalid interval.");
            Thread.Sleep(1000);
            return;
        }

        var baseDir = AppContext.BaseDirectory;
        var outputDir = Path.Combine(baseDir, "ScreenCaptures");

        Directory.CreateDirectory(outputDir);

        foreach (var file in Directory.GetFiles(outputDir))
            File.Delete(file);

        Console.WriteLine("\nCapturing screen (DEBUG MODE)...");
        Console.WriteLine("Press Q to stop.\n");

        // Keyboard is platform-aware
        IScreenCapture screen =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsScreenCapture()
                : new MacScreenCapture();

        int index = 1;

        while (true)
        {
            if (Console.KeyAvailable &&
                Console.ReadKey(true).Key == ConsoleKey.Q)
                break;

            var buffer = screen.Capture(out int w, out int h);

            var filePath = Path.Combine(
                outputDir,
                $"capture_{index:D5}.bmp"
            );

            BmpWriter.SaveBGRA(buffer, w, h, filePath);

            Console.WriteLine($"Captured {Path.GetFileName(filePath)}");

            index++;
            //Thread.Sleep(intervalSeconds * 1000);
        }
    }

    static async Task ShowScriptsMenu()
    {
        Console.Clear();

        var baseDir = AppContext.BaseDirectory;

        var approvedPath = Path.Combine(baseDir, "Scripts");
        var customPath = Path.Combine(baseDir, "Scripts_Custom");

        var approvedScripts = ScriptLoader.LoadScriptDescriptors(approvedPath);
        var customScripts = ScriptLoader.LoadScriptDescriptors(customPath);

        var menuMap = new List<(bool IsHeader, string Text, object? Script)>();

        if (approvedScripts.Count > 0)
        {
            menuMap.Add((true, "=== Official Scripts ===", null));

            foreach (var s in approvedScripts)
                menuMap.Add((false, s.Title, s));
        }

        if (customScripts.Count > 0)
        {
            if (menuMap.Count > 0)
                menuMap.Add((true, "", null));

            menuMap.Add((true, "=== Custom Scripts ===", null));

            foreach (var s in customScripts)
                menuMap.Add((false, s.Title, s));
        }

        if (menuMap.All(m => m.IsHeader))
        {
            Console.WriteLine("No scripts found.");
            Thread.Sleep(1500);
            return;
        }

        int displayIndex = 1;
        var indexMap = new Dictionary<int, dynamic>();

        Console.WriteLine("Available scripts:\n");

        foreach (var item in menuMap)
        {
            if (item.IsHeader)
            {
                Console.WriteLine(item.Text);
            }
            else
            {
                Console.WriteLine($"{displayIndex}) {item.Text}");
                indexMap[displayIndex] = item.Script;
                displayIndex++;
            }
        }

        Console.Write("\nSelect script number (or B to go back): ");
        var input = Console.ReadLine();

        if (string.Equals(input, "b", StringComparison.OrdinalIgnoreCase))
            return;

        if (!int.TryParse(input, out int selection) ||
            !indexMap.ContainsKey(selection))
        {
            Console.WriteLine("Invalid selection.");
            Thread.Sleep(1500);
            return;
        }

        var selectedMeta = indexMap[selection];
        var selected = ScriptLoader.LoadFullScript(selectedMeta.FilePath);

        Console.Clear();
        Console.WriteLine(selected.Title);
        Console.WriteLine(new string('-', selected.Title.Length));
        Console.WriteLine(selectedMeta.Description);
        Console.WriteLine("\nPress ENTER to run, or Q to cancel.");

        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Q)
            return;

        if (selected.Script == null)
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

        await engine.RunScriptAsync(selected.Script);
    }


}

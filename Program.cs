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

class Program
{
    static async Task Main()
    {
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
                    Thread.Sleep(1000);
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
        var scriptsPath = Path.Combine(baseDir, "Scripts");

        var scripts = ScriptLoader.LoadScripts(scriptsPath);

        if (scripts.Count == 0)
        {
            Console.WriteLine("No scripts found.");
            Thread.Sleep(1500);
            return;
        }

        Console.WriteLine("Available scripts:\n");

        for (int i = 0; i < scripts.Count; i++)
            Console.WriteLine($"{i + 1}) {scripts[i].Title}");

        Console.Write("\nSelect script number (or B to go back): ");
        var input = Console.ReadLine();

        if (string.Equals(input, "b", StringComparison.OrdinalIgnoreCase))
            return;

        if (!int.TryParse(input, out int index) ||
            index < 1 ||
            index > scripts.Count)
        {
            Console.WriteLine("Invalid selection.");
            Thread.Sleep(1500);
            return;
        }

        var selected = scripts[index - 1];

        Console.Clear();
        Console.WriteLine(selected.Title);
        Console.WriteLine(new string('-', selected.Title.Length));
        Console.WriteLine(selected.Description);
        Console.WriteLine("\nPress ENTER to run, or Q to cancel.");

        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Q)
            return;

        if (selected.Run == null)
        {
            Console.WriteLine("No Run section defined.");
            Thread.Sleep(1500);
            return;
        }


        // Keyboard is platform-aware
        IScreenCapture screen =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsScreenCapture()
                : new MacScreenCapture();


        // Keyboard is platform-aware
        IKeyboard keyboard =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsKeyboard()
                : new MacKeyboard();

        var runtime = new AutomationRuntime(screen, keyboard);
        var engine = new AutomationEngine(runtime);

        await engine.RunScriptAsync(selected.Run);
    }
}

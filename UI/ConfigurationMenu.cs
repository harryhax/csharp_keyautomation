using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Csharp_GTA_KeyAutomation.ImageCapture;
using Csharp_GTA_KeyAutomation.Screen;

namespace Csharp_GTA_KeyAutomation.UI;

public static class ConfigurationMenu
{
    public static void Show()
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
    private static void RunScreenCaptureLoop()
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
        }
    }
}

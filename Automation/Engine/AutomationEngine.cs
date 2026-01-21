using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Csharp_GTA_KeyAutomation.Automation.Models;
using Csharp_GTA_KeyAutomation.ImageCapture;
using Csharp_GTA_KeyAutomation.ImageCompare;
using Csharp_GTA_KeyAutomation.Input;

namespace Csharp_GTA_KeyAutomation.Automation.Engine;

public class AutomationEngine
{
    private readonly AutomationRuntime _ctx;

    private readonly Dictionary<string, (byte[] Pixels, int W, int H)> _templateCache
        = new();

    public AutomationEngine(AutomationRuntime ctx)
    {
        _ctx = ctx;
    }

    public async Task RunScriptAsync(AutomationScript script)
    {
        // SETUP (runs once)
        if (script.Setup != null)
        {
            foreach (var step in script.Setup)
                await ExecuteStep(step);
        }

        if (script.Loop == null || script.Loop.Steps.Count == 0)
            return;

        // PROMPT ONCE BEFORE LOOP
        int repeatCount = ResolveRepeatCount(script.Loop.Repeat);

        Console.WriteLine();
        Console.WriteLine($"Loop will run {repeatCount} time(s).");
        Console.WriteLine();

        for (int i = 0; i < repeatCount; i++)
        {
            Console.WriteLine($"--- Iteration {i + 1}/{repeatCount} ---");

            foreach (var step in script.Loop.Steps)
                await ExecuteStep(step);
        }
    }

    private int ResolveRepeatCount(ScriptRepeat? repeat)
    {
        if (repeat == null)
            return 1;

        if (repeat.Mode == "fixed")
            return repeat.Default;

        if (repeat.Mode == "prompt")
        {
            Console.Write($"Enter repeat count (default {repeat.Default}): ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value > 0)
                return value;

            return repeat.Default;
        }

        throw new InvalidOperationException($"Unknown repeat mode: {repeat.Mode}");
    }

    private async Task ExecuteStep(AutomationStep step)
    {
        switch (step.Type)
        {
            case "instruction":
            {
                Console.ForegroundColor = ConsoleColor.Green;

                if (!string.IsNullOrWhiteSpace(step.Description))
                    Console.WriteLine(step.Description);

                Console.ResetColor();

                Console.WriteLine();
                int promptLine = Console.CursorTop;
                Console.Write("Press ENTER to continue...");
                Console.ReadLine();

                int width = Console.WindowWidth;
                Console.SetCursorPosition(0, promptLine);
                Console.Write(new string(' ', width));
                Console.SetCursorPosition(0, promptLine);
                break;
            }

            case "tap":
                KeyboardHelpers.TapName(_ctx.Keyboard, step.Key!, step.HoldMs);
                if (step.SleepAfterMs > 0)
                    await Task.Delay(step.SleepAfterMs);
                break;

            case "keydown":
                KeyboardHelpers.KeyDownName(_ctx.Keyboard, step.Key!);
                break;

            case "keyup":
                KeyboardHelpers.KeyUpName(_ctx.Keyboard, step.Key!);
                break;

            case "wait":
            {
                int remainingMs = step.Ms;

                while (remainingMs > 0)
                {
                    int remainingSeconds =
                        (int)Math.Ceiling(remainingMs / 1000.0);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"\r[WAIT] {remainingSeconds}s remaining...   ");

                    int delay = Math.Min(1000, remainingMs);
                    await Task.Delay(delay);
                    remainingMs -= delay;
                }

                Console.ResetColor();
                Console.Write("\r[WAIT] Done.                    \n");
                break;
            }

            case "loop":
                for (int i = 0; i < step.Count; i++)
                {
                    foreach (var inner in step.Steps!)
                        await ExecuteStep(inner);
                }
                break;

            case "waitForImage":
                await WaitForImage(step);
                break;

            default:
                throw new InvalidOperationException($"Unknown step type: {step.Type}");
        }
    }

    private async Task WaitForImage(AutomationStep step)
    {
        if (string.IsNullOrWhiteSpace(step.Template))
            throw new InvalidOperationException("Template path is required");

        if (!_templateCache.TryGetValue(step.Template, out var cached))
        {
            if (!File.Exists(step.Template))
                throw new FileNotFoundException(step.Template);

            var pixels = BmpLoader.Load(
                step.Template,
                out int templateWidth,
                out int templateHeight
            );

            cached = (pixels, templateWidth, templateHeight);
            _templateCache[step.Template] = cached;
        }

        var (template, tw, th) = cached;

        for (int attempt = 1; attempt <= step.MaxAttempts; attempt++)
        {
            var frame = _ctx.Screen.Capture(out int w, out int h);

            double matchPct = 0;
            long ms = 0;

            if (w == tw && h == th)
            {
                (matchPct, ms) =
                    PixelDiff.CompareRGBA(frame, template, w, h);

                if (matchPct >= step.MinMatch)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(
                        $"\r{DateTime.Now:HH:mm:ss} | " +
                        $"Matched {Path.GetFileName(step.Template)} " +
                        $"{matchPct:F2}% | {ms}ms\n"
                    );
                    Console.ResetColor();
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(
                $"\r{DateTime.Now:HH:mm:ss} | " +
                $"Matching {Path.GetFileName(step.Template)} " +
                $"{matchPct:F2}% / {step.MinMatch:F2}% | {ms}ms"
            );
            Console.ResetColor();

            await Task.Delay(step.PollDelayMs);
        }

        Console.WriteLine("\nwaitForImage: timeout reached");
    }
}

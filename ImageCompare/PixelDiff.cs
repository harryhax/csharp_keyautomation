using System;
using System.Diagnostics;

namespace Csharp_GTA_KeyAutomation.ImageCompare;

public static class PixelDiff
{
    // threshold: 0–255
    public static (double matchPercent, long elapsedMs) CompareRGBA(
        byte[] a,
        byte[] b,
        int width,
        int height,
        byte threshold = 25
    )
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Buffers must be same length");

        var sw = Stopwatch.StartNew();

        int diffPixels = 0;
        int totalPixels = width * height;

        for (int i = 0; i < a.Length; i += 4)
        {
            int dr = Math.Abs(a[i]     - b[i]);
            int dg = Math.Abs(a[i + 1] - b[i + 1]);
            int db = Math.Abs(a[i + 2] - b[i + 2]);
            int da = Math.Abs(a[i + 3] - b[i + 3]);

            int maxDiff = Math.Max(Math.Max(dr, dg), Math.Max(db, da));

            if (maxDiff > threshold)
                diffPixels++;
        }

        sw.Stop();

        double matchPercent =
            ((totalPixels - diffPixels) / (double)totalPixels) * 100.0;

        return (matchPercent, sw.ElapsedMilliseconds);
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace Csharp_GTA_KeyAutomation.Screen;

public sealed class MacScreenCapture : IScreenCapture
{
    private readonly string _tmpDir =
        Path.Combine(Path.GetTempPath(), "csharp-screencapture");

    private byte[]? _buffer;
    private int _bufW, _bufH;

    public MacScreenCapture()
    {

        Directory.CreateDirectory(_tmpDir);
    }

    public byte[] Capture(out int width, out int height)
    {
        width = 0;
        height = 0;

        try
        {
            string path = Path.Combine(
                _tmpDir,
                $"capture_{Guid.NewGuid():N}.bmp"
            );

            var psi = new ProcessStartInfo
            {
                FileName = "/usr/sbin/screencapture",
                Arguments = $"-x -t bmp \"{path}\"",
                UseShellExecute = false
            };

            using (var p = Process.Start(psi))
            {
                p!.WaitForExit();
                if (p.ExitCode != 0)
                    throw new Exception("screencapture failed");
            }

            byte[] bmp = File.ReadAllBytes(path);
            File.Delete(path);

            DecodeBmpToBGRA(bmp, out width, out height);
            return _buffer!;
        }
        catch (Exception ex)
        {
            Console.WriteLine("MacScreenCapture");
            Console.WriteLine(ex);
            throw new Exception("screencapture failed");
        }
    }

    private void DecodeBmpToBGRA(byte[] bmp, out int width, out int height)
    {
        // BMP header offsets
        int pixelOffset = BitConverter.ToInt32(bmp, 10);
        width = BitConverter.ToInt32(bmp, 18);
        height = BitConverter.ToInt32(bmp, 22);

        bool bottomUp = height > 0;
        height = Math.Abs(height);

        int bytesPerPixel = 4;
        int srcRowStride = ((width * bytesPerPixel + 3) / 4) * 4;
        int dstRowStride = width * bytesPerPixel;

        if (_buffer == null || _bufW != width || _bufH != height)
        {
            _buffer = new byte[width * height * 4];
            _bufW = width;
            _bufH = height;
        }

        for (int y = 0; y < height; y++)
        {
            int srcY = bottomUp ? (height - 1 - y) : y;

            Buffer.BlockCopy(
                bmp,
                pixelOffset + srcY * srcRowStride,
                _buffer,
                y * dstRowStride,
                dstRowStride
            );
        }
    }

}

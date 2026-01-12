using System;
using System.IO;

namespace Csharp_GTA_KeyAutomation.ImageCapture;

/// <summary>
/// Writes raw BGRA pixel buffers to 32-bit BMP files.
/// </summary>
static class BmpWriter
{
    /// <summary>
    /// Saves a BGRA buffer as a BMP to an explicit file path.
    /// The caller is fully responsible for directory selection.
    /// </summary>
    public static void SaveBGRA(
        byte[] buffer,
        int width,
        int height,
        string outputPath
    )
    {
        // Ensure the target directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        SaveBGRAInternal(buffer, width, height, outputPath);
    }

    /// <summary>
    /// Saves a BGRA buffer as a BMP to a directory + filename.
    /// Convenience overload for callers that separate concerns.
    /// </summary>
    public static void SaveBGRA(
        byte[] buffer,
        int width,
        int height,
        string outputDirectory,
        string fileName
    )
    {
        // Normalize filename only (prevents traversal)
        fileName = Path.GetFileName(fileName);

        Directory.CreateDirectory(outputDirectory);

        string path = Path.Combine(outputDirectory, fileName);

        SaveBGRAInternal(buffer, width, height, path);
    }

    /// <summary>
    /// Core BMP writer implementation.
    /// Assumes BGRA input and writes a 32-bit uncompressed BMP.
    /// </summary>
    private static void SaveBGRAInternal(
        byte[] buffer,
        int width,
        int height,
        string path
    )
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var bw = new BinaryWriter(fs);

        int bytesPerPixel = 4;
        int rowSize = width * bytesPerPixel;
        int imageSize = rowSize * height;
        int fileSize = 54 + imageSize;

        // --- BMP FILE HEADER (14 bytes) ---
        bw.Write((byte)'B');
        bw.Write((byte)'M');
        bw.Write(fileSize);
        bw.Write(0);      // reserved
        bw.Write(54);     // pixel data offset

        // --- DIB HEADER (BITMAPINFOHEADER, 40 bytes) ---
        bw.Write(40);         // header size
        bw.Write(width);
        bw.Write(height);
        bw.Write((short)1);   // planes
        bw.Write((short)32);  // bits per pixel
        bw.Write(0);          // BI_RGB (no compression)
        bw.Write(imageSize);
        bw.Write(0);          // x pixels per meter
        bw.Write(0);          // y pixels per meter
        bw.Write(0);          // colors used
        bw.Write(0);          // important colors

        // --- PIXEL DATA ---
        // BMP rows are written bottom-up; buffer is assumed BGRA.
        for (int y = height - 1; y >= 0; y--)
        {
            int rowStart = y * rowSize;
            bw.Write(buffer, rowStart, rowSize);
        }
    }
}

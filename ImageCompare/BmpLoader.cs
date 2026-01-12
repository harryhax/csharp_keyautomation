using System;
using System.IO;
namespace Csharp_GTA_KeyAutomation.ImageCapture;

public static class BmpLoader
{
    public static byte[] Load(string path, out int width, out int height)
    {
        byte[] file = File.ReadAllBytes(path);

        width = BitConverter.ToInt32(file, 18);
        height = BitConverter.ToInt32(file, 22);
        int dataOffset = BitConverter.ToInt32(file, 10);

        int bytesPerPixel = 4;
        int stride = width * bytesPerPixel;

        byte[] buffer = new byte[width * height * bytesPerPixel];

        for (int y = 0; y < height; y++)
        {
            int src = dataOffset + ((height - 1 - y) * stride);
            int dst = y * stride;
            Buffer.BlockCopy(file, src, buffer, dst, stride);
        }

        return buffer;
    }
}

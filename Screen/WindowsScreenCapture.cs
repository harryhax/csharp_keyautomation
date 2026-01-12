

using System;
using System.Runtime.InteropServices;

namespace Csharp_GTA_KeyAutomation.Screen;

sealed class WindowsScreenCapture : IScreenCapture
{

    public WindowsScreenCapture()
    {
        Console.WriteLine("WindowsScreenCapture");

    }
    public byte[] Capture(out int width, out int height)
    {
        width = 0;
        height = 0;

        try
        {
            width = GetSystemMetrics(0);  // SM_CXSCREEN
            height = GetSystemMetrics(1); // SM_CYSCREEN

            int stride = width * 4;
            byte[] buffer = new byte[stride * height];

            IntPtr hScreen = GetDC(IntPtr.Zero);
            IntPtr hDC = CreateCompatibleDC(hScreen);
            IntPtr hBitmap = CreateCompatibleBitmap(hScreen, width, height);
            SelectObject(hDC, hBitmap);

            BitBlt(hDC, 0, 0, width, height, hScreen, 0, 0, SRCCOPY);

            BITMAPINFO bmi = new()
            {
                bmiHeader = new BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
                    biWidth = width,
                    biHeight = -height, // top-down
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = BI_RGB
                }
            };

            GetDIBits(hDC, hBitmap, 0, (uint)height, buffer, ref bmi, DIB_RGB_COLORS);

            DeleteObject(hBitmap);
            DeleteDC(hDC);
            ReleaseDC(IntPtr.Zero, hScreen);

            return buffer;

        }
        catch (Exception ex)
        {
            Console.WriteLine("MacScreenCapture");
            Console.WriteLine(ex);
            throw new Exception("screencapture failed");
        }
    }

    private const int SRCCOPY = 0x00CC0020;
    private const int BI_RGB = 0;
    private const int DIB_RGB_COLORS = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(
        IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest,
        IntPtr hdcSrc, int xSrc, int ySrc, int rop);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")]
    private static extern int GetDIBits(
        IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines,
        byte[] lpvBits, ref BITMAPINFO lpbmi, uint uUsage);
}


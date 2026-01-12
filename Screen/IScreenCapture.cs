namespace Csharp_GTA_KeyAutomation.Screen;

public interface IScreenCapture
{

    byte[] Capture(out int width, out int height);
}

using System;
using System.Threading.Tasks;
using Csharp_GTA_KeyAutomation.Infrastructure.Updates;

namespace Csharp_GTA_KeyAutomation.UI;

public static class UpdateMenu
{
    public static async Task ShowAsync()
    {
        Console.Clear();
        Console.WriteLine("Check for Updates");
        Console.WriteLine("-----------------\n");

        try
        {
            var result = await UpdateChecker.CheckAsync();

            Console.WriteLine($"Current version: {result.LocalVersion}");

            if (result.RemoteVersion == null)
            {
                Console.WriteLine("Unable to determine latest version.");
            }
            else
            {
                Console.WriteLine($"Latest version : {result.RemoteVersion}");

                if (result.UpdateAvailable)
                {
                    Console.WriteLine("\nUpdate available.");

                    if (!string.IsNullOrWhiteSpace(result.ReleaseName))
                        Console.WriteLine($"Release: {result.ReleaseName}");

                    if (!string.IsNullOrWhiteSpace(result.ReleaseNotes))
                    {
                        Console.WriteLine("\nRelease notes:");
                        Console.WriteLine("--------------------------------");
                        Console.WriteLine(result.ReleaseNotes);
                    }
                }
                else
                {
                    Console.WriteLine("\nYou are running the latest version.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update check failed:");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("\n--------------------------------");
        Console.WriteLine("Press ENTER to return.");
        Console.ReadLine();
    }
}

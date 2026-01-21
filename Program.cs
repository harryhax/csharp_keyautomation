
using Csharp_GTA_KeyAutomation.Automation.Scripts;
using Csharp_GTA_KeyAutomation.UI;


class Program
{
    static async Task Main()
    {
        ScriptRepositorySync.BaseUrl =
          "https://raw.githubusercontent.com/harryhax/harrykey_scripts/main/";

        ScriptRepositorySync.Debug = false;

        var baseDir = AppContext.BaseDirectory;

        Console.WriteLine("Syncing scripts...\n");

        try
        {
            await ScriptRepositorySync.SyncAsync(baseDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Script sync failed:");
            Console.WriteLine(ex.Message);
            Console.WriteLine("\nPress ENTER to continue anyway.");
            Console.ReadLine();
        }
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Main Menu");
            Console.WriteLine("--------------------------------");

            Console.WriteLine("1) Scripts");
            Console.WriteLine("2) Configuration");
            Console.WriteLine("3) Check for Updates");

            Console.WriteLine("--------------------------------");
            Console.WriteLine("q) Quit");
            Console.WriteLine("--------------------------------\n");

            Console.Write("Select option: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                return;

            switch (input)
            {
                case "1":
                    await ScriptMenu.ShowAsync();
                    break;

                case "2":
                    ConfigurationMenu.Show();
                    break;

                case "3":
                    await UpdateMenu.ShowAsync();
                    break;


                default:
                    Console.WriteLine("Invalid selection.");
                    Thread.Sleep(1000);
                    break;
            }
        }

    }
}
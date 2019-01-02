using FbonizziMonoGameWindowsDesktop;
using System;
using System.Globalization;

namespace Rellow.WindowsDesktop
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new RellowBootstrap(
                textFileAssetsLoader: new WindowsTextFileImporter(),
                settingsRepository: new FileWindowsSettingsRepository("rellow-settings.txt"),
                webPageOpener: new WindowsWebSiteOpener(),
                gameCulture: CultureInfo.CreateSpecificCulture("it-IT"),
                isPc: true,
                isFullScreen: false,
                rateMeUri: new Uri("https://www.fbonizzi.it"),
                deviceWidth: 450, deviceHeight: 800))
            {
                game.Run();
            }
        }
    }
#endif
}

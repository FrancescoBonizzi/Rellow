using Android.App;
using Android.Content.PM;
using Android.Views;
using FbonizziMonoGame.PlatformAbstractions;
using FbonizziMonoGameAndroid;
using System;
using System.Globalization;

namespace Rellow.Android
{
    [Activity(
        Label = "Rellow",
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        MainLauncher = true,
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize)]
    public class MainActivity : FbonizziMonoGameActivity
    {
        private RellowBootstrap _game;

        protected override IFbonizziGame StartGame(CultureInfo gameCulture)
        {
            _game = new RellowBootstrap(
                textFileAssetsLoader: new AndroidTextFileImporter(Assets),
                settingsRepository: new AndroidSettingsRepository(this),
                webPageOpener: new AndroidWebPageOpener(this),
                gameCulture: gameCulture,
                isPc: false,
                isFullScreen: true,
                rateMeUri: new Uri("market://details?id=com.francescobonizzi.rellow"));

            _game.Run();
            SetContentView((View)_game.Services.GetService(typeof(View)));

            return _game;
        }
        
        protected override void DisposeGame()
        {
            _game?.Dispose();
            _game = null;
        }
    }
}


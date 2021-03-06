﻿using FbonizziMonoGame.Assets;
using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Drawing.Abstractions;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.Input;
using FbonizziMonoGame.Input.Abstractions;
using FbonizziMonoGame.PlatformAbstractions;
using FbonizziMonoGame.Sprites;
using FbonizziMonoGame.StringsLocalization;
using FbonizziMonoGame.StringsLocalization.Abstractions;
using FbonizziMonoGame.UI.RateMe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rellow.Assets;
using Rellow.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rellow
{
    public class RellowBootstrap : Game, IFbonizziGame
    {
        private const string _gameName = "Rellow";

        private enum RunningStates
        {
            Splashscreen,
            Running
        }

        private readonly Uri _rateMeUri;
        private RunningStates _currentState;

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        private SpriteBatch _spriteBatch;

        private SplashScreenLoader _splashScreenAssetsLoader;

        private readonly ITextFileLoader _textFileAssetsLoader;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWebPageOpener _webPageOpener;
        private List<IInputListener> _inputListeners;

        private readonly CultureInfo _gameCulture;

        public event EventHandler ExitGameRequested;

        private AssetsLoader _assetsLoader;
        private IScreenTransformationMatrixProvider _matrixScaleProvider;
 
        private Sprite _mousePointer;

        private GameOrchestrator _orchestrator;
        private SoundManager _soundManager;
        private readonly bool _isPc;

        private readonly ILocalizedStringsRepository _localizedStringsRepository;

        // Just for MonoGame.Framework.WindowsUniversal bootstrapping requirement
        public RellowBootstrap() { }

        public RellowBootstrap(
           ITextFileLoader textFileAssetsLoader,
           ISettingsRepository settingsRepository,
           IWebPageOpener webPageOpener,
           CultureInfo gameCulture,
           bool isPc,
           bool isFullScreen,
           Uri rateMeUri,
           int? deviceWidth = null,
           int? deviceHeight = null)
        {
            _isPc = isPc;
         
            Window.Title = _gameName;

            _rateMeUri = rateMeUri;
            _currentState = RunningStates.Splashscreen;

            _textFileAssetsLoader = textFileAssetsLoader;
            _settingsRepository = settingsRepository;
            _webPageOpener = webPageOpener;
            _gameCulture = gameCulture;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown,
                IsFullScreen = isFullScreen
            };

            if (deviceWidth != null && deviceHeight != null)
            {
                GraphicsDeviceManager.PreferredBackBufferWidth = deviceWidth.Value;
                GraphicsDeviceManager.PreferredBackBufferHeight = deviceHeight.Value;
            }

            _localizedStringsRepository = new InMemoryLocalizedStringsRepository(new Dictionary<string, string>());
        }

        protected override void Initialize()
        {
            _matrixScaleProvider = new DynamicScalingMatrixProvider(
                  new GameWindowScreenSizeChangedNotifier(Window),
                  GraphicsDeviceManager.GraphicsDevice,
                  1080, 1920,
                  true);
            base.Initialize();
        }

        private void LoadGameAssets()
        {
            new GameStringsLoader(_localizedStringsRepository, _gameCulture);

            _assetsLoader = new AssetsLoader(
               Content,
               _textFileAssetsLoader);
            _mousePointer = _assetsLoader.Sprites["manina"];
            _soundManager = new SoundManager(_assetsLoader);

            var gameFactory = new Func<RellowGame>(
                () => new RellowGame(
                    _matrixScaleProvider,
                    _assetsLoader,
                    _orchestrator,
                    _soundManager,
                    _settingsRepository,
                    _localizedStringsRepository));

            var dialogDefinition = new Rectangle(
                    100, 550, _matrixScaleProvider.VirtualWidth - 200, _matrixScaleProvider.VirtualHeight / 3);
            var rateMeDialog = new RateMeDialog(
                launchesUntilPrompt: 2,
                maxRateShowTimes: 2,
                rateAppUri: _rateMeUri,
                dialogDefinition: dialogDefinition,
                font: _assetsLoader.WritingFont,
                localizedStringsRepository: _localizedStringsRepository,
                rateMeDialogStrings: _gameCulture.TwoLetterISOLanguageName == "it" ? (RateMeDialogStrings)new DefaultItalianRateMeDialogStrings(_gameName) : (RateMeDialogStrings)new DefaultEnglishRateMeDialogStrings(_gameName),
                webPageOpener: _webPageOpener,
                settingsRepository: _settingsRepository,
                buttonADefinition: new Rectangle(
                    dialogDefinition.X + dialogDefinition.Width / 2 - 250,
                    dialogDefinition.Y + 350,
                    500, 100),
                buttonBDefinition: new Rectangle(
                    dialogDefinition.X + dialogDefinition.Width / 2 - 250,
                    dialogDefinition.Y + 500,
                    500, 100),
                backgroundColor: Color.DarkGray.WithAlpha(1f),
                buttonsBackgroundColor: Definitions.PrimaryBackgroundColor.WithAlpha(1f),
                buttonsShadowColor: Color.Black,
                backgroundShadowColor: Color.Black.WithAlpha(1f),
                titleColor: Color.Black,
                buttonsTextColor: Definitions.PrimaryForegroundColor,
                titlePositionOffset: new Vector2(dialogDefinition.Width / 2, 80f),
                buttonTextPadding: 60f,
                titlePadding: 90f);

            var mainMenuFactory = new Func<MainMenuPage>(
                () => new MainMenuPage(
                    _assetsLoader,
                    _orchestrator,
                    _matrixScaleProvider,
                    _localizedStringsRepository,
                    rateMeDialog,
                    _settingsRepository));

            _orchestrator = new GameOrchestrator(
                gameFactory,
                mainMenuFactory,
                GraphicsDevice,
                _matrixScaleProvider,
                _soundManager,
                _webPageOpener);

            _inputListeners = new List<IInputListener>();

            if (_isPc)
            {
                var mouseListener = new MouseListener(_matrixScaleProvider);
                mouseListener.MouseDown += MouseListener_MouseClicked;
                _inputListeners.Add(mouseListener);
            }
            else
            {
                var touchListener = new TouchListener(_matrixScaleProvider);
                touchListener.TouchStarted += TouchListener_TouchEnded;

                var gamepadListener = new GamePadListener();
                gamepadListener.ButtonDown += GamepadListener_ButtonDown;

                _inputListeners.Add(touchListener);
                _inputListeners.Add(gamepadListener);
            }

            // Perchè il back di Android lo prende la tastiera
            var keyboardListener = new KeyboardListener();
            keyboardListener.KeyPressed += KeyboardListener_KeyPressed;
            _inputListeners.Add(keyboardListener);
        }

        private void GamepadListener_ButtonDown(object sender, GamePadEventArgs e)
        {
            if (e.Button == Buttons.Back)
            {
                _orchestrator.Back();
                if (_orchestrator.ShouldEndApplication)
                {
                    if (ExitGameRequested != null)
                        ExitGameRequested(this, EventArgs.Empty); // Se ho un handler specifico, uso quello
                    else
                        Exit(); // Devono ancora fixare il problema dell'uscita da Android
                }
            }
        }

        private void KeyboardListener_KeyPressed(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Keys.Escape)
            {
                _orchestrator.Back();
                if (_orchestrator.ShouldEndApplication)
                {
                    if (ExitGameRequested != null)
                        ExitGameRequested(this, EventArgs.Empty); // Se ho un handler specifico, uso quello
                    else
                        Exit(); // Devono ancora fixare il problema dell'uscita da Android
                }
            }
            else if (e.Key == Keys.Back) // L'indietro di Android viene triggerato qui!
            {
                _orchestrator.Back();
                if (_orchestrator.ShouldEndApplication)
                {
                    if (ExitGameRequested != null)
                        ExitGameRequested(this, EventArgs.Empty); // Se ho un handler specifico, uso quello
                    else
                        Exit(); // Devono ancora fixare il problema dell'uscita da Android
                }
            }
        }

        private void TouchListener_TouchEnded(object sender, TouchEventArgs e)
            => _orchestrator.HandleTouchInput(e.Position);

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
            => _orchestrator.HandleTouchInput(e.Position);

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _splashScreenAssetsLoader = new SplashScreenLoader(
                LoadGameAssets,
                Content,
                "Splashscreen");
            _splashScreenAssetsLoader.Load();

            _splashScreenAssetsLoader.Completed += _splashScreenAssetsLoader_Completed;
        }

        private void _splashScreenAssetsLoader_Completed(object sender, EventArgs e)
        {
            _splashScreenAssetsLoader = null;
            _orchestrator.Start();
            _currentState = RunningStates.Running;
        }

        public void Pause()
            => _orchestrator?.Pause();

        public void Resume()
            => _orchestrator?.Resume();

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            var elapsed = gameTime.ElapsedGameTime;

            if (_currentState == RunningStates.Splashscreen)
            {
                _splashScreenAssetsLoader.Update(elapsed);
                return;
            }

            foreach (var listener in _inputListeners)
                listener.Update(gameTime);

            _orchestrator.Update(elapsed);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (_currentState == RunningStates.Splashscreen)
            {
                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.Begin(transformMatrix: _matrixScaleProvider.ScaleMatrix);
                _splashScreenAssetsLoader.Draw(_spriteBatch);
                _spriteBatch.End();
                return;
            }

            _orchestrator.Draw(_spriteBatch, GraphicsDevice);

            if (_isPc)
            {
                if (!_orchestrator.IsPaused)
                {
                    var mouseState = Mouse.GetState();
                    var mousePosition = new Vector2(mouseState.X - 32, mouseState.Y);
                    if (mouseState.X != 0 && mouseState.Y != 0)
                    {
                        _spriteBatch.Begin();
                        _spriteBatch.Draw(_mousePointer.Sheet, mousePosition, _mousePointer.SourceRectangle, Color.White);
                        _spriteBatch.End();
                    }
                }
            }

            base.Draw(gameTime);
        }

    }
}

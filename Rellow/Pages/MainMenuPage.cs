using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Drawing.Abstractions;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.PlatformAbstractions;
using FbonizziMonoGame.StringsLocalization.Abstractions;
using FbonizziMonoGame.UI.RateMe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rellow.Assets;
using Rellow.UI;
using System;

namespace Rellow.Pages
{
    public class MainMenuPage
    {
        private readonly Color _backgroundColor = Definitions.PrimaryBackgroundColor;
        private readonly Color _foregroundColor = Definitions.PrimaryForegroundColor;

        private readonly string _scoreText;
        private readonly Color _scoreColor = Color.White;
        private readonly DrawingInfos _scoreDrawingInfos;

        private readonly SpriteFont _writingFont;
        private readonly RateMeDialog _rateMeDialog;
        private readonly Rectangle _backgroundRectangle;

        private readonly FloatingText _title;
        private readonly FloatingTextButton _playButton;
        private readonly FloatingTextButton _aboutButton;

        private readonly SoundManager _soundManager;

        private readonly IScreenTransformationMatrixProvider _matrixScaleProvider;

        public MainMenuPage(
            AssetsLoader assetsLoader,
            SoundManager soundManager,
            GameOrchestrator gameOrchestrator,
            IScreenTransformationMatrixProvider matrixScaleProvider,
            ILocalizedStringsRepository localizedStringsRepository,
            RateMeDialog rateMeDialog,
            ISettingsRepository settingsRepository)
        {
            _soundManager = soundManager;
            _matrixScaleProvider = matrixScaleProvider;
            _backgroundRectangle = new Rectangle(0, 0, matrixScaleProvider.VirtualWidth, matrixScaleProvider.VirtualHeight);

            var titleFont = assetsLoader.TitleFont;
            _writingFont = assetsLoader.WritingFont;
            _rateMeDialog = rateMeDialog ?? throw new ArgumentNullException(nameof(rateMeDialog));

            var titleText = "RELLOW";
            _title = new FloatingText(
                titleText,
                new DrawingInfos()
                {
                    Position = new Vector2(matrixScaleProvider.VirtualWidth / 2f, 250f),
                    Origin = titleFont.GetTextCenter(titleText),
                    OverlayColor = _foregroundColor
                },
                titleFont);

            var playText = localizedStringsRepository.Get(GameStringsLoader.PlayStringKey);
            _playButton = new FloatingTextButton(
                playText,
                new DrawingInfos()
                {
                    Position = new Vector2(matrixScaleProvider.VirtualWidth / 2f, 850f),
                    Origin = _writingFont.GetTextCenter(playText),
                    OverlayColor = _foregroundColor
                },
                _writingFont,
                () =>
                {
                    gameOrchestrator.SetGameState();
                });

            var aboutText = localizedStringsRepository.Get(GameStringsLoader.AboutStringKey);
            _aboutButton = new FloatingTextButton(
                aboutText,
                new DrawingInfos()
                {
                    Position = new Vector2(matrixScaleProvider.VirtualWidth / 2f, 1150f),
                    Origin = _writingFont.GetTextCenter(aboutText),
                    OverlayColor = _foregroundColor
                },
                _writingFont,
                () =>
                {
                    gameOrchestrator.SetAboutState();
                });

            var currentScore = settingsRepository.GetOrSetInt(Definitions.SCORE_KEY, 0);

            _scoreText = $"{localizedStringsRepository.Get(GameStringsLoader.ScoreStringKey)}:{currentScore}";
            _scoreDrawingInfos = new DrawingInfos()
            {
                Position = new Vector2(matrixScaleProvider.VirtualWidth / 2f, matrixScaleProvider.VirtualHeight - 250f - 200f),
                Origin = _writingFont.GetTextCenter(_scoreText),
                OverlayColor = _scoreColor,
                Scale = 0.5f
            };
        }

        public void HandleInput(Vector2 touchPoint)
        {
            if (_rateMeDialog.ShouldShowDialog)
            {
                _rateMeDialog.HandleInput(touchPoint);
            }
            else // Per non far sì che si possano premere i pulsanti sotto gli altri
            {
                _playButton.HandleInput(touchPoint);
                _aboutButton.HandleInput(touchPoint);
            }
        }

        public void Update(TimeSpan elapsed)
        {
            _title.Update(elapsed);
            _playButton.Update(elapsed);
            _aboutButton.Update(elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: _matrixScaleProvider.ScaleMatrix);
            spriteBatch.DrawRectangle(_backgroundRectangle, _backgroundColor);
            _title.Draw(spriteBatch);
            _playButton.Draw(spriteBatch);
            _aboutButton.Draw(spriteBatch);
            spriteBatch.DrawString(_writingFont, _scoreText, _scoreDrawingInfos);

            if (_rateMeDialog.ShouldShowDialog)
                _rateMeDialog.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}

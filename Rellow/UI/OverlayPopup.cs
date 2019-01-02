using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Drawing.Abstractions;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.StringsLocalization.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rellow.Assets;
using System;

namespace Rellow.UI
{
    public class OverlayPopup
    {
        private readonly Rectangle _backgroundRectangle;
        private readonly Color _backgroundColor;

        private readonly Rectangle _popupRectangle;
        private readonly Rectangle _popupRectangleShadow;
        private readonly Color _popupBackgroundColor;
        private readonly Color _popupShadowBackgroundColor;

        private readonly FloatingTextButton _playAgainButton;
        private readonly FloatingTextButton _exitButton;

        private readonly SpriteFont _font;
        private string TitleText { get; set; }
        private readonly DrawingInfos _titleDrawingInfos;

        private string Text { get; }
        private readonly DrawingInfos _textDrawingInfos;


        public OverlayPopup(
            string title,
            string text,
            SpriteFont font,
            IScreenTransformationMatrixProvider matrixScaleProvider,
            ILocalizedStringsRepository localizedStringsRepository,
            Action playAgainFunction,
            Action exitFunction)
        {
            _font = font;

            _backgroundRectangle = new Rectangle(0, 0, matrixScaleProvider.VirtualWidth, matrixScaleProvider.VirtualHeight);
            _backgroundColor = Color.Gray.WithAlpha(0.7f);

            int popupRectangleWidth = 900;
            _popupRectangle = new Rectangle(
                (matrixScaleProvider.VirtualWidth - popupRectangleWidth) / 2, 260,
                popupRectangleWidth, popupRectangleWidth + 200);
            _popupBackgroundColor = Definitions.PrimaryBackgroundColor.WithAlpha(1f);

            _popupRectangleShadow = new Rectangle(
                _popupRectangle.X + 15,
                _popupRectangle.Y + 15,
                _popupRectangle.Width,
                _popupRectangle.Height);
            _popupShadowBackgroundColor = Color.Black.WithAlpha(1f);

            _titleDrawingInfos = new DrawingInfos()
            {
                Position = new Vector2(
                    matrixScaleProvider.VirtualWidth / 2,
                    _popupRectangle.Y + _popupRectangle.Height / 4),
                Origin = _font.GetTextCenter(title),
                OverlayColor = Color.Black
            };

            _textDrawingInfos = new DrawingInfos()
            {
                Position = _titleDrawingInfos.Position + new Vector2(0f, 200f),
                Origin = _font.GetTextCenter(text),
                Scale = 0.6f,
                OverlayColor = Color.White
            };

            TitleText = title;
            Text = text;

            _playAgainButton = new FloatingTextButton(
                localizedStringsRepository.Get(GameStringsLoader.PlayAgainStringKey),
                new DrawingInfos()
                {
                    Position = _textDrawingInfos.Position + new Vector2(0f, 250f),
                    Origin = _font.GetTextCenter(localizedStringsRepository.Get(GameStringsLoader.PlayAgainStringKey)),
                    OverlayColor = Definitions.PrimaryForegroundColor,
                },
                _font,
                playAgainFunction,
                0.7f, 0.8f);

            _exitButton = new FloatingTextButton(
                localizedStringsRepository.Get(GameStringsLoader.ExitStringKey),
                new DrawingInfos()
                {
                    Position = _textDrawingInfos.Position + new Vector2(0f, 450f),
                    Origin = _font.GetTextCenter(localizedStringsRepository.Get(GameStringsLoader.ExitStringKey)),
                    OverlayColor = Definitions.PrimaryForegroundColor
                },
                _font,
                exitFunction,
                0.7f, 0.8f);
        }

        public void HandleInput(Vector2 touchPoint)
        {
            _playAgainButton.HandleInput(touchPoint);
            _exitButton.HandleInput(touchPoint);
        }

        public void Update(TimeSpan elapsed)
        {
            _playAgainButton.Update(elapsed);
            _exitButton.Update(elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(_backgroundRectangle, _backgroundColor);
            spriteBatch.DrawRectangle(_popupRectangleShadow, _popupShadowBackgroundColor);
            spriteBatch.DrawRectangle(_popupRectangle, _popupBackgroundColor);

            spriteBatch.DrawString(_font, TitleText, _titleDrawingInfos);
            spriteBatch.DrawString(_font, Text, _textDrawingInfos);
            _playAgainButton.Draw(spriteBatch);
            _exitButton.Draw(spriteBatch);
        }
    }
}

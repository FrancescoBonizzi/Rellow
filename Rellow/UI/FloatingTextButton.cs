using FbonizziMonoGame.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rellow.UI
{
    public class FloatingTextButton : FloatingText
    {
        private readonly Action _action;
        private readonly Vector2 _textSize;

        public FloatingTextButton(
            string text,
            DrawingInfos drawingInfos,
            SpriteFont font,
            Action onClick,
            float minScale = 1.0f,
            float maxScale = 1.2f)
            : base(text, drawingInfos, font, minScale, maxScale)
        {
            _action = onClick;
            _textSize = font.MeasureString(text);
        }

        public void HandleInput(Vector2 touchPoint)
        {
            if (_drawingInfos.HitBox((int)_textSize.X, (int)_textSize.Y)
              .Contains(touchPoint))
            {
                _action();
            }
        }
    }
}

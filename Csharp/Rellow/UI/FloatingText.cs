using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.TransformationObjects;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rellow.UI
{
    public class FloatingText
    {
        private readonly ScalingObject _scalingObject;
        private readonly SpriteFont _font;
        protected readonly DrawingInfos _drawingInfos;
        private readonly string _text;

        public FloatingText(
            string text,
            DrawingInfos drawingInfos,
            SpriteFont font,
            float minScale = 1.0f,
            float maxScale = 1.2f)
        {
            _text = text;
            _drawingInfos = drawingInfos;
            _scalingObject = new ScalingObject(minScale, maxScale, 1.0f);
            _font = font;
        }

        public void Update(TimeSpan elapsed)
        {
            _scalingObject.Update(elapsed);
            _drawingInfos.Scale = _scalingObject.Scale;
        }

        public void Draw(SpriteBatch spriteBatch)
            => spriteBatch.DrawString(_font, _text, _drawingInfos);
    }
}

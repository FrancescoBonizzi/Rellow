using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rellow.Assets;
using System;

namespace Rellow.UI
{
    public class GameButton
    {
        private readonly Sprite _buttonBottom;
        private readonly Sprite _buttonUpPressed;
        private readonly Sprite _buttonUpReleased;
        
        private readonly DrawingInfos _bottomPartDrawingInfos;
        private readonly DrawingInfos _upPartDrawingInfos;
        private Rectangle _collisionRectangle;

        private readonly int _squaredButtonSize;

        private readonly TimeSpan _autoReleaseTime = TimeSpan.FromMilliseconds(150);
        private TimeSpan? _autoReleaseTimer;

        public ColorWithName ColorWithName { get; }

        private enum ButtonState
        {
            Down,
            Up
        }

        private ButtonState _currentState;

        public GameButton(
            AssetsLoader assetsLoader,
            int squaredButtonSize,
            ColorWithName color)
        {
            _buttonBottom = assetsLoader.Sprites["button-bottom"];
            _buttonUpPressed = assetsLoader.Sprites["button-up-pressed"];
            _buttonUpReleased = assetsLoader.Sprites["button-up-released"];

            _squaredButtonSize = squaredButtonSize;

            _upPartDrawingInfos = new DrawingInfos()
            {
                OverlayColor = color.ColorGraphic
            };

            _bottomPartDrawingInfos = new DrawingInfos()
            {
                OverlayColor = Color.White
            };

            ColorWithName = color;
            _currentState = ButtonState.Up;
        }

        public void SetPosition(Vector2 position)
        {
            _upPartDrawingInfos.Position = position;
            _bottomPartDrawingInfos.Position = position;
            _collisionRectangle = new Rectangle(
                (int)position.X, (int)position.Y,
                _squaredButtonSize, _squaredButtonSize);
        }

        public bool IsTouched(Vector2 touchPosition)
        {
            if (_collisionRectangle.Contains(touchPosition))
            {
                _currentState = ButtonState.Down;
                _autoReleaseTimer = _autoReleaseTime;
                return true;
            }
            else
            {
                _currentState = ButtonState.Up;
            }

            return false;
        }

        public void Update(TimeSpan elapsed)
        {
            if (_autoReleaseTimer < TimeSpan.Zero)
            {
                _currentState = ButtonState.Up;
                _autoReleaseTimer = null;

            }
            else if (_autoReleaseTimer >= TimeSpan.Zero)
            {
                _autoReleaseTimer -= elapsed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_buttonBottom, _upPartDrawingInfos);

            switch (_currentState)
            {
                case ButtonState.Down:
                    spriteBatch.Draw(_buttonUpPressed, _upPartDrawingInfos);
                    break;

                case ButtonState.Up:
                    spriteBatch.Draw(_buttonUpReleased, _upPartDrawingInfos);
                    break;
            }
        }

    }
}

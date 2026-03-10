using FbonizziMonoGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rellow.UI
{
    public class ProgressBar
    {
        private enum ProgressBarDirection
        {
            LoadingRight,
            LoadingLeft,
            LoadingUp,
            LoadingDown
        }

        private ProgressBarDirection _currentProgressBarDirection;

        private Rectangle _barRectangle;
        public float MaxValue { get; set; }
        private float _currentValue;

        public Color Color { get; set; }
        private readonly int _widthWhenComplete;
        private readonly int _heightWhenComplete;

        public ProgressBar(
            Rectangle positionAndSize,
            Color color,
            float maxValue)
        {
            _barRectangle = positionAndSize;
            _widthWhenComplete = positionAndSize.Width;
            _heightWhenComplete = positionAndSize.Height;
            _currentValue = 0;
            Color = color;
            _currentProgressBarDirection = ProgressBarDirection.LoadingUp;
            MaxValue = maxValue;
        }

        public void Reset()
        {
            switch (_currentProgressBarDirection)
            {
                case ProgressBarDirection.LoadingRight:
                    _currentProgressBarDirection = ProgressBarDirection.LoadingDown;
                    _barRectangle.Height = 0;
                    _barRectangle.Width = _widthWhenComplete;

                    break;

                case ProgressBarDirection.LoadingLeft:
                    _currentProgressBarDirection = ProgressBarDirection.LoadingUp;
                    _barRectangle.Height = _heightWhenComplete;
                    _barRectangle.Width = _widthWhenComplete;

                    break;

                case ProgressBarDirection.LoadingUp:
                    _currentProgressBarDirection = ProgressBarDirection.LoadingRight;
                    _barRectangle.Height = _heightWhenComplete;
                    _barRectangle.Width = _widthWhenComplete;

                    break;

                case ProgressBarDirection.LoadingDown:
                    _currentProgressBarDirection = ProgressBarDirection.LoadingLeft;
                    _barRectangle.Height = _heightWhenComplete;
                    _barRectangle.Width = _widthWhenComplete;
                    
                    break;
            }
        }

        public void SetValue(float value)
        {
            if (value > MaxValue)
                throw new ArgumentException("value cannot be > maxValue");

            _currentValue = value;
        }

        public void Update(TimeSpan elapsed)
        {
            var completionPercentage = _currentValue / MaxValue;

            switch (_currentProgressBarDirection)
            {
                case ProgressBarDirection.LoadingRight:
                    _barRectangle.Width = (int)(_widthWhenComplete * completionPercentage);
                    break;

                case ProgressBarDirection.LoadingLeft:
                    _barRectangle.Width = (int)(_widthWhenComplete * (1f - completionPercentage));
                    break;

                case ProgressBarDirection.LoadingUp:
                    _barRectangle.Height = (int)(_heightWhenComplete * (1f - completionPercentage));
                    break;

                case ProgressBarDirection.LoadingDown:
                    _barRectangle.Height = (int)(_heightWhenComplete * completionPercentage);
                    break;
            }

            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(_barRectangle, Color);
        }
    }
}

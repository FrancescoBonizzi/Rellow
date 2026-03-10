using FbonizziMonoGame.Extensions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Rellow.UI
{
    public class UIColors
    {
        public Color TimerBarColor { get; private set; }
        public Color ScoreColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color CurrentWordBackgroundColor { get; private set; }

        private readonly Color[] _availableColors;

        public UIColors(Color[] availableColors)
        {
            _availableColors = availableColors;
            ShuffleUIColors();

            CurrentWordBackgroundColor = Color.Black;
        }

        /// <summary>
        /// To distract the player
        /// </summary>
        public void ShuffleUIColors()
        {
            _availableColors.Shuffle();
            var colorsToPick = new Queue<Color>(_availableColors);

            TimerBarColor = colorsToPick.Dequeue();
            ScoreColor = colorsToPick.Dequeue();
            BackgroundColor = colorsToPick.Dequeue();
        }
    }
}

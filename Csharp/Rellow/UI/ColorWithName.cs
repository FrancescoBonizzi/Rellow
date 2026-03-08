using Microsoft.Xna.Framework;

namespace Rellow.UI
{
    public class ColorWithName
    {
        private readonly string _primaryColorName;
        private readonly string _secondaryColorName;

        public string ColorName { get; private set; }
        public Color ColorGraphic { get; }

        public ColorWithName(
            Color colorGraphic,
            string colorName,
            string secondaryColorName)
        {
            ColorGraphic = colorGraphic;
            _primaryColorName = colorName;
            _secondaryColorName = secondaryColorName;
            ShuffleColorName();
        }

        public void ShuffleColorName()
        {
            ColorName = FbonizziMonoGame.Numbers.RandomBetween(0D, 1D) > 0.5D
                ? _primaryColorName
                : _secondaryColorName;
        }
    }
}

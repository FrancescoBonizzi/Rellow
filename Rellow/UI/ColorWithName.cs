using Microsoft.Xna.Framework;

namespace Rellow.UI
{
    public class ColorWithName
    {
        public Color ColorGraphic { get; }
        public string ColorName { get; }

        public ColorWithName(Color colorGraphic, string colorName)
        {
            ColorGraphic = colorGraphic;
            ColorName = colorName;
        }
    }
}

using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.StringsLocalization.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rellow.Assets;
using Rellow.UI;
using System;
using System.Linq;

namespace Rellow
{
    public class GameButtonsManager
    {
        private readonly GameButton[] _buttons;
        private int _activeButtonsNumber;
        private readonly Vector2 _startingPosition;
        private const int _totalNumberOfButtons = 9;
        private static readonly Random _random = new Random();
        private const int _squaredButtonSize = 250;
        private const int _padding = 100;

        public ColorWithName[] SpawnableColors { get; }
        public Color[] ButtonColors { get; }
        public Color CurrentWordForegroundColor { get; private set; }
        public string CurrentWord { get; private set; }

        public event EventHandler OnWon;
        public event EventHandler OnLost;

        public GameButtonsManager(
            AssetsLoader assetsLoader,
            ILocalizedStringsRepository localizedStringsRepository,
            Vector2 startingPosition)
        {
            _buttons = new GameButton[_totalNumberOfButtons];
            _activeButtonsNumber = 3;
            _startingPosition = startingPosition;

            SpawnableColors = new ColorWithName[]
            {
                new ColorWithName(
                    Color.Yellow,
                    localizedStringsRepository.Get(GameStringsLoader.YellowColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageYellowColorKey)),

                new ColorWithName(
                    Color.Red,
                    localizedStringsRepository.Get(GameStringsLoader.RedColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageRedColorKey)),

                new ColorWithName(
                    Color.Blue,
                    localizedStringsRepository.Get(GameStringsLoader.BlueColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageBlueColorKey)),

                new ColorWithName(
                    new Color(0, 255, 0),
                    localizedStringsRepository.Get(GameStringsLoader.GreenColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageGreenColorKey)),

                new ColorWithName(
                    new Color(255, 115, 0),
                    localizedStringsRepository.Get(GameStringsLoader.OrangeColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageOrangeColorKey)),

                new ColorWithName(
                    new Color(255, 0, 255),
                    localizedStringsRepository.Get(GameStringsLoader.VioletColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageVioletColorKey)),

                new ColorWithName(
                    new Color(150, 150, 150),
                    localizedStringsRepository.Get(GameStringsLoader.GrayColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageGrayColorKey)),

                new ColorWithName(
                    Color.White,
                    localizedStringsRepository.Get(GameStringsLoader.WhiteColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageWhiteColorKey)),

                new ColorWithName(
                    new Color(0, 255, 255),
                    localizedStringsRepository.Get(GameStringsLoader.LightBlueColorKey),
                    localizedStringsRepository.Get(GameStringsLoader.SecondaryLanguageLightBlueColorKey))
            };

            for (int c = 0; c < 3; ++c)
            {
                for (int r = 0; r < 3; ++r)
                {
                    int index = (c * 3) + r;
                    _buttons[index] = new GameButton(
                        assetsLoader,
                        _squaredButtonSize,
                        SpawnableColors[index]);
                }
            }
        }

        public void HandleInput(Vector2 touchPosition)
        {
            foreach (var button in _buttons)
            {
                if (button.IsTouched(touchPosition))
                {
                    if (button.ColorWithName.ColorName == CurrentWord)
                    {
                        OnWon?.Invoke(null, EventArgs.Empty);
                        return;
                    }
                    else
                    {
                        OnLost?.Invoke(null, EventArgs.Empty);
                        return;
                    }
                }
            }
        }

        public void NewRound()
        {
            // Mescolo i pulsanti con i colori
            _buttons.Shuffle();

            var pickableButtons = _buttons.Take(_activeButtonsNumber).ToList();

            // Scelgo un colore all'interno di questi
            int pickedColorIndex = _random.Next(0, pickableButtons.Count);

            _buttons[pickedColorIndex]
                .ColorWithName
                .ShuffleColorName();

            CurrentWordForegroundColor = _buttons[pickedColorIndex]
                .ColorWithName
                .ColorGraphic;

            // Per non prendere mai lo stesso colore della parola...
            //pickableButtons.RemoveAt(pickedColorIndex);
            // ...ma si può fare anche senza, per creare un po' di confusione

            // Scelgo il nome di un colore tra quelli dei pulsanti attivi
            CurrentWord = pickableButtons[_random.Next(0, pickableButtons.Count)]
                .ColorWithName
                .ColorName;

            for (int c = 0; c < 3; ++c)
            {
                for (int r = 0; r < 3; ++r)
                {
                    int index = (c * 3) + r;

                    if (index > _activeButtonsNumber)
                    {
                        break;
                    }

                    _buttons[index].SetPosition(new Vector2(
                        _startingPosition.X + (r * _squaredButtonSize) + (_padding * r),
                        _startingPosition.Y + (c * _squaredButtonSize) + (_padding * c)));
                }
            }
        }

        public void Update(TimeSpan elapsed)
        {
            foreach (var b in _buttons)
            {
                b.Update(elapsed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int b = 0; b < _activeButtonsNumber; ++b)
            {
                _buttons[b].Draw(spriteBatch);
            }
        }

        public void AddButton()
            => _activeButtonsNumber = (_activeButtonsNumber + 1) % (_totalNumberOfButtons + 1);

        public bool CanAddButtons
            => _activeButtonsNumber < _totalNumberOfButtons;
    }
}

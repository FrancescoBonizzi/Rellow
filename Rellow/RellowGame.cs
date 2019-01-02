using FbonizziMonoGame.Drawing;
using FbonizziMonoGame.Drawing.Abstractions;
using FbonizziMonoGame.Extensions;
using FbonizziMonoGame.PlatformAbstractions;
using FbonizziMonoGame.StringsLocalization.Abstractions;
using FbonizziMonoGame.TransformationObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rellow.Assets;
using Rellow.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rellow
{
    public class RellowGame
    {
        private enum GameStates
        {
            PlayingWaitingForInput,
            AfterWonWait,
            Lost
        }

        private class PopupText
        {
            public string Text { get; set; }
            public DrawingInfos DrawingInfos { get; set; }
            public PopupObject PopupObject { get; set; }
        }

        private TimeSpan _currentWordTimer;
        private TimeSpan _choiceTime;
        private TimeSpan _currentWaitingBetweenRoundsTimer;
        private readonly TimeSpan _timeBetweenRounds = TimeSpan.FromMilliseconds(150);
        private readonly IScreenTransformationMatrixProvider _matrixScaleProvider;
        private readonly ILocalizedStringsRepository _localizedStringsRepository;
        private readonly GameButtonsManager _gameButtonsManager;
        private readonly GameOrchestrator _gameOrchestrator;
        private readonly ISettingsRepository _settingsRepository;
        private GameStates _currentGameState;

        private readonly Rectangle _backgroundRectangle;
        private readonly Rectangle _colorTextBackgroundRectangle;

        private readonly SpriteFont _writingFont;

        private readonly string _scoreTextTitle;
        private string _scoreText;
        private DrawingInfos _scoreTextDrawingInfos;
        private int _score;

        private readonly UIColors _gameColors;
        private DrawingInfos _currentWordDrawingInfos;
        
        private bool _isPaused = false;

        private int _numberOfVictories;
        private const int _numberOfVictoriesToIncreaseDifficulty = 5;

        private OverlayPopup _gameOverOverlayPopup;
        private SoundManager _soundManager;
        private readonly ProgressBar _timeProgressBar;

        private List<PopupText> _scoreDifferentialPopups;
        private TimeSpan _popupScoreDuration = TimeSpan.FromSeconds(2);

        public RellowGame(
            IScreenTransformationMatrixProvider matrixScaleProvider,
            AssetsLoader assetsLoader,
            GameOrchestrator gameOrchestrator,
            SoundManager soundManager,
            ISettingsRepository settingsRepository,
            ILocalizedStringsRepository localizedStringsRepository)
        {
            _localizedStringsRepository = localizedStringsRepository;

            _gameButtonsManager = new GameButtonsManager(
                 assetsLoader,
                 localizedStringsRepository,
                 new Vector2(60f, 800f));

            _scoreDifferentialPopups = new List<PopupText>();

            _soundManager = soundManager ?? throw new ArgumentNullException(nameof(soundManager));
            _gameOrchestrator = gameOrchestrator ?? throw new ArgumentNullException(nameof(gameOrchestrator));
            _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));

            _currentGameState = GameStates.PlayingWaitingForInput;
            _gameColors = new UIColors(_gameButtonsManager.SpawnableColors.Select(c => c.ColorGraphic).ToArray());
            _matrixScaleProvider = matrixScaleProvider ?? throw new ArgumentNullException(nameof(matrixScaleProvider));

            _choiceTime = TimeSpan.FromSeconds(2.5);

            _backgroundRectangle = new Rectangle(0, 0, matrixScaleProvider.VirtualWidth, matrixScaleProvider.VirtualHeight);
     
            _scoreTextTitle = localizedStringsRepository.Get(GameStringsLoader.ScoreStringKey);

            _gameButtonsManager = new GameButtonsManager(
                assetsLoader,
                localizedStringsRepository,
                new Vector2(60f, 720f));
            _gameButtonsManager.OnLost += _gameButtonsManager_OnLost;
            _gameButtonsManager.OnWon += _gameButtonsManager_OnWon;

            _writingFont = assetsLoader.WritingFont;

            _colorTextBackgroundRectangle = new Rectangle(
                0, 500,
                matrixScaleProvider.VirtualWidth,
                120);

            _numberOfVictories = 0;
            _score = 0;

            _timeProgressBar = new ProgressBar(
                new Rectangle(
                    0, 0,
                    _matrixScaleProvider.VirtualWidth,
                    _matrixScaleProvider.VirtualHeight),
                _gameColors.TimerBarColor,
                (int)_choiceTime.TotalMilliseconds);

            UpdateScoreString();
            NewRound();
        }

        private void UpdateScoreString()
        {
            _scoreText = $"{_scoreTextTitle}:{_score}";

            _scoreTextDrawingInfos = new DrawingInfos()
            {
                Position = new Vector2(_matrixScaleProvider.VirtualWidth / 2f, _colorTextBackgroundRectangle.Y / 3),
                Origin = _writingFont.GetTextCenter(_scoreText),
                Scale = 0.8f,
                OverlayColor = _gameColors.ScoreColor
            };
        }

        private void UpdateTimerProgressBar()
        {
            _timeProgressBar.SetValue((int)((_choiceTime - _currentWordTimer).TotalMilliseconds));
        }

        private void NewRound()
        {
            if (IsTimeToShuffleColors())
            {
                _gameColors.ShuffleUIColors();
            }

            if (_numberOfVictories > 0)
            {
                var differential = (_numberOfVictories * 10) + (int)((_choiceTime - _currentWordTimer).TotalMilliseconds / 10);
                _score += differential;

                var popupText = $"+{differential}";
                var startingPopupPosition = _scoreTextDrawingInfos.Position + new Vector2(0f, 230f);
                var scoreDifferentialPopup = new PopupText()
                {
                    Text = popupText,
                    DrawingInfos = new DrawingInfos()
                    {
                        Position = startingPopupPosition,
                        Scale = 1.0f,
                        Origin = _writingFont.GetTextCenter(popupText),
                    },
                    PopupObject = new PopupObject(
                        _popupScoreDuration,
                        startingPopupPosition,
                        _gameColors.ScoreColor,
                        110)
                };
                scoreDifferentialPopup.PopupObject.Popup();

                _scoreDifferentialPopups.Add(scoreDifferentialPopup);
            }

            ++_numberOfVictories;

            _currentWordTimer = _choiceTime;
            _gameButtonsManager.NewRound();

            _currentWordDrawingInfos = new DrawingInfos()
            {
                Position = new Vector2(
                    _matrixScaleProvider.VirtualWidth / 2f,
                    14f + _colorTextBackgroundRectangle.Y + _colorTextBackgroundRectangle.Height / 2f),
                Origin = _writingFont.GetTextCenter(_gameButtonsManager.CurrentWord),
                OverlayColor = _gameButtonsManager.CurrentWordForegroundColor
            };

            if (_gameButtonsManager.CanAddButtons
                && IsTimeToIncreaseDifficulty())
            {
                _choiceTime -= TimeSpan.FromMilliseconds(200);
                _gameButtonsManager.AddButton();
            }

            _timeProgressBar.MaxValue = (int)_choiceTime.TotalMilliseconds;
            _timeProgressBar.Color = _gameColors.TimerBarColor;

            UpdateScoreString();
            _timeProgressBar.Reset();
            UpdateTimerProgressBar();
            _currentGameState = GameStates.PlayingWaitingForInput;
        }

        private void _gameButtonsManager_OnWon(object sender, EventArgs e)
        {
            _currentGameState = GameStates.AfterWonWait;
            _currentWaitingBetweenRoundsTimer = _timeBetweenRounds;
            _soundManager.PlayWin();
        }

        public void Pause()
            => _isPaused = true;

        public void Resume()
            => _isPaused = false;

        private bool IsTimeToIncreaseDifficulty()
            => _numberOfVictories % _numberOfVictoriesToIncreaseDifficulty == 0;

        private bool IsTimeToShuffleColors()
        {
            return true;
        }

        private void _gameButtonsManager_OnLost(object sender, EventArgs e)
            => HandleLost();

        private void HandleLost()
        {
            _soundManager.PlayLoose();
            var currentRecord = _settingsRepository.GetOrSetInt(Definitions.SCORE_KEY, 0);

            var scoreTextGameover = $"{_localizedStringsRepository.Get(GameStringsLoader.ScoreStringKey)}:{_score}";

            if (_score > currentRecord)
            {
                _settingsRepository.SetInt(Definitions.SCORE_KEY, _score);
                scoreTextGameover = $"{scoreTextGameover}\nRECORD!";
            }

            _currentGameState = GameStates.Lost;
            _gameOverOverlayPopup = new OverlayPopup(
                "GAME OVER",
                scoreTextGameover,
                _writingFont,
                _matrixScaleProvider,
                _localizedStringsRepository,
                () => _gameOrchestrator.Replay(),
                () => _gameOrchestrator.SetMenuState());
        }

        public void HandleInput(Vector2 touchPosition)
        {
            switch (_currentGameState)
            {
                case GameStates.PlayingWaitingForInput:
                    _gameButtonsManager.HandleInput(touchPosition);
                    break;

                case GameStates.AfterWonWait:
                    break;

                case GameStates.Lost:
                    _gameOverOverlayPopup?.HandleInput(touchPosition);
                    break;
            }
        }

        public void Update(TimeSpan elapsed)
        {
            if (_isPaused)
                return;

            for (int p = _scoreDifferentialPopups.Count - 1; p >= 0; --p)
            {
                var scoreDifferentialPopup = _scoreDifferentialPopups[p];
                scoreDifferentialPopup.PopupObject.Update(elapsed);
                scoreDifferentialPopup.DrawingInfos.Position = scoreDifferentialPopup.PopupObject.Position;
                scoreDifferentialPopup.DrawingInfos.OverlayColor = scoreDifferentialPopup.PopupObject.OverlayColor;

                if (scoreDifferentialPopup.PopupObject.IsCompleted)
                    _scoreDifferentialPopups.RemoveAt(p);
            }

            switch (_currentGameState)
            {
                case GameStates.PlayingWaitingForInput:

                    _currentWordTimer -= elapsed;
                    _gameButtonsManager.Update(elapsed);
                    _timeProgressBar.Update(elapsed);

                    if (_currentWordTimer <= TimeSpan.Zero)
                    {
                        HandleLost();
                    }

                    UpdateTimerProgressBar();
                    break;

                case GameStates.AfterWonWait:

                    _currentWaitingBetweenRoundsTimer -= elapsed;
                    _gameButtonsManager.Update(elapsed);
                    _timeProgressBar.Update(elapsed);

                    if (_currentWaitingBetweenRoundsTimer <= TimeSpan.Zero)
                    {
                        NewRound();
                    }

                    break;

                case GameStates.Lost:

                    _gameOverOverlayPopup.Update(elapsed);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: _matrixScaleProvider.ScaleMatrix);
            spriteBatch.DrawRectangle(_backgroundRectangle, _gameColors.BackgroundColor);
            _timeProgressBar.Draw(spriteBatch);
            spriteBatch.DrawRectangle(_colorTextBackgroundRectangle, _gameColors.CurrentWordBackgroundColor);
            spriteBatch.DrawString(_writingFont, _scoreText.ToString(), _scoreTextDrawingInfos);
            spriteBatch.DrawString(_writingFont, _gameButtonsManager.CurrentWord, _currentWordDrawingInfos);
            foreach (var scoreDifferentialPopup in _scoreDifferentialPopups)
                spriteBatch.DrawString(_writingFont, scoreDifferentialPopup.Text, scoreDifferentialPopup.DrawingInfos);
            _gameButtonsManager.Draw(spriteBatch);

            if (_currentGameState == GameStates.Lost)
            {
                _gameOverOverlayPopup.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

    }
}

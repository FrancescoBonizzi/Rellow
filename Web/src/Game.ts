import { Application, Container, Graphics, Ticker } from 'pixi.js';
import { RellowAssets } from './assets/RellowAssets';
import SoundManager from './services/SoundManager';
import ScoreRepository from './services/ScoreRepository';
import { ColorButtonGrid } from './ui/ColorButtonGrid';
import { WordDisplay } from './ui/WordDisplay';
import { ProgressBar } from './ui/ProgressBar';
import { ScoreText } from './ui/ScoreText';
import { GAME_COLORS, shuffleUIColors } from './ui/colors';

const INITIAL_CHOICE_TIME_MS  = 2500;
const TIME_REDUCTION_MS       = 200;
const MIN_CHOICE_TIME_MS      = 500;
const PAUSE_BETWEEN_ROUNDS    = 150;
const VICTORIES_TO_DIFFICULTY = 5;

type GameState = 'waiting' | 'after_won' | 'lost';

class Game {
    private readonly _stage: Container;
    private readonly _background: Graphics;
    private readonly _progressBar: ProgressBar;
    private readonly _colorButtonGrid: ColorButtonGrid;
    private readonly _wordDisplay: WordDisplay;
    private readonly _scoreText: ScoreText;
    private readonly _soundManager: SoundManager;
    private _state: GameState = 'waiting';
    private _choiceTime: number = INITIAL_CHOICE_TIME_MS;
    private _timer: number = INITIAL_CHOICE_TIME_MS;
    private _pauseTimer: number = 0;
    private _numberOfVictories: number = 0;
    private _score: number = 0;

    constructor(
        assets: RellowAssets,
        app: Application,
        soundManager: SoundManager,
    ) {
        this._soundManager = soundManager;
        this._stage = new Container();
        app.stage.addChild(this._stage);

        // Z-order 1: background
        this._background = new Graphics();
        this._background.rect(0, 0, 1080, 1920).fill(0xff1243);
        this._stage.addChild(this._background);

        // Z-order 2: progress bar (sopra sfondo)
        this._progressBar = new ProgressBar(this._stage, INITIAL_CHOICE_TIME_MS, 0xffffff);

        // Z-order 3: griglia bottoni
        this._colorButtonGrid = new ColorButtonGrid(this._stage, assets);
        this._colorButtonGrid.onWon  = () => this._onWon();
        this._colorButtonGrid.onLost = () => this._handleLost();

        // Z-order 4: word display
        this._wordDisplay = new WordDisplay(this._stage, assets);

        // Z-order 5: score text
        this._scoreText = new ScoreText(this._stage, assets);

        // Round iniziale
        this._newRound();
    }

    private _newRound(): void {
        // 1. Shuffle colori UI ogni round
        const uiColors = shuffleUIColors(GAME_COLORS);
        this._background.clear().rect(0, 0, 1080, 1920).fill(uiColors.backgroundColor);
        this._progressBar.setColor(uiColors.timerBarColor);
        this._scoreText.setColor(uiColors.scoreTextColor);

        // 2. Calcola punteggio differenziale (solo dopo il primo round)
        if (this._numberOfVictories > 0) {
            const diff = (this._numberOfVictories * 10) + Math.floor(this._timer / 10);
            this._score += diff;
            this._scoreText.showPopup(diff, uiColors.scoreTextColor);
        }

        // 3. Incrementa vittorie
        this._numberOfVictories++;

        // 4. Aumenta difficoltà ogni 5 vittorie
        if (this._numberOfVictories % VICTORIES_TO_DIFFICULTY === 0) {
            this._choiceTime = Math.max(MIN_CHOICE_TIME_MS, this._choiceTime - TIME_REDUCTION_MS);
            if (this._colorButtonGrid.canAddButtons)
                this._colorButtonGrid.addButton();
        }

        // 5. Reset timer
        this._timer = this._choiceTime;

        // 6. Aggiorna ProgressBar
        this._progressBar.setMaxValue(this._choiceTime);
        this._progressBar.reset();
        this._progressBar.setValue(this._timer);

        // 7. Nuovo round visivo (shuffle bottoni + nuova parola)
        const roundSetup = this._colorButtonGrid.setupNewRound();
        this._wordDisplay.setWord(roundSetup.wordText, roundSetup.wordForegroundColor);

        // 8. Aggiorna display punteggio
        this._scoreText.setScore(this._score);

        // 9. Riabilita input e torna in waiting
        this._colorButtonGrid.setInputEnabled(true);
        this._state = 'waiting';
    }

    private _onWon(): void {
        this._state = 'after_won';
        this._pauseTimer = PAUSE_BETWEEN_ROUNDS;
        this._colorButtonGrid.setInputEnabled(false);
        this._soundManager.playOk();
    }

    private _handleLost(): void {
        if (this._state === 'lost') return;
        this._state = 'lost';
        this._colorButtonGrid.setInputEnabled(false);
        this._soundManager.playWrong();

        ScoreRepository.setScore('gameover', this._score);
        if (ScoreRepository.isNewRecord(this._score))
            ScoreRepository.setScore('record', this._score);

        import('./pages/router').then(m => m.default.navigate('/gameover'));
    }

    update(time: Ticker): void {
        this._colorButtonGrid.update(time.deltaMS);
        this._scoreText.update(time.deltaMS);

        switch (this._state) {
            case 'waiting':
                this._timer -= time.deltaMS;
                this._progressBar.setValue(Math.max(0, this._timer));
                if (this._timer <= 0) this._handleLost();
                break;
            case 'after_won':
                this._pauseTimer -= time.deltaMS;
                if (this._pauseTimer <= 0) this._newRound();
                break;
            case 'lost':
                break;
        }
    }

    destroy(): void {
        this._colorButtonGrid.destroy();
        this._wordDisplay.destroy();
        this._scoreText.destroy();
        this._progressBar.destroy();
        this._stage.destroy({ children: true });
    }
}

export default Game;

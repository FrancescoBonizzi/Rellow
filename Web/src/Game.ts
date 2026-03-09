import { Application, Container, Graphics, Ticker } from 'pixi.js';
import { RellowAssets } from './assets/RellowAssets';
import SoundManager from './services/SoundManager';
import { ColorButtonGrid } from './ui/ColorButtonGrid';
import { WordDisplay } from './ui/WordDisplay';

class Game {
    private readonly _stage: Container;
    private readonly _background: Graphics;
    private readonly _colorButtonGrid: ColorButtonGrid;
    private readonly _wordDisplay: WordDisplay;

    constructor(
        assets: RellowAssets,
        app: Application,
        _soundManager: SoundManager,
    ) {
        this._stage = new Container();
        app.stage.addChild(this._stage);

        this._background = new Graphics();
        this._background.rect(0, 0, 1080, 1920).fill(0xff1243);
        this._stage.addChild(this._background);

        this._colorButtonGrid = new ColorButtonGrid(this._stage, assets);
        this._colorButtonGrid.onWon  = () => this._onWon();
        this._colorButtonGrid.onLost = () => this._onLost();

        this._wordDisplay = new WordDisplay(this._stage, assets);

        const roundSetup = this._colorButtonGrid.setupNewRound();
        this._wordDisplay.setWord(roundSetup.wordText, roundSetup.wordForegroundColor);
    }

    private _onWon(): void {
        // Fase 6: punteggio, pausa 150ms, difficoltà, setupNewRound
    }

    private _onLost(): void {
        // Fase 6: navigate('/gameover')
    }

    update(time: Ticker): void {
        this._colorButtonGrid.update(time.deltaMS);
        // Fase 6: timer decrescente, stati, shuffle UI colors
    }

    destroy(): void {
        this._colorButtonGrid.destroy();
        this._wordDisplay.destroy();
        this._stage.destroy({ children: true });
    }
}

export default Game;

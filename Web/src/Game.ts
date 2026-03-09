import { Application, Ticker } from 'pixi.js';
import { RellowAssets } from './assets/RellowAssets';
import SoundManager from './services/SoundManager';

class Game {
    constructor(
        _assets: RellowAssets,
        _app: Application,
        _soundManager: SoundManager,
    ) {
        // Fase 6: inizializzazione componenti di gioco
    }

    update(_time: Ticker): void {
        // Fase 6: game loop
    }

    destroy(): void {
        // Fase 6: cleanup componenti
    }
}

export default Game;

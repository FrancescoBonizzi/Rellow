import { Application } from 'pixi.js';
import { loadAssets } from '../assets/AssetsLoader';
import LoadingThing from '../uiKit/LoadingThing';
import Game from '../Game';
import { SoundManagerInstance } from '../services/SoundInstance';

const GAME_W = 1080;
const GAME_H = 1920;

let app: Application | null = null;
let gameContainer: HTMLDivElement | null = null;
let resizeObserver: ResizeObserver | null = null;

export async function initGame(container: HTMLElement): Promise<void> {
    if (app) destroyGame();

    container.innerHTML = `
    <div id="game-wrapper">
        <div id="game-container"></div>
    </div>`;

    gameContainer = container.querySelector<HTMLDivElement>('#game-container');
    if (!gameContainer) {
        console.error('Game container non trovato');
        return;
    }

    app = new Application();
    await app.init({
        background: '#ff1243',
        width: GAME_W,
        height: GAME_H,
        premultipliedAlpha: false,
        antialias: false,
        autoDensity: true,
        resolution: Math.min(window.devicePixelRatio || 1, 2),
    });

    try { await (screen.orientation as unknown as { lock: (o: string) => Promise<void> }).lock('portrait'); } catch { /* non supportato su tutti i browser */ }

    resizeObserver = new ResizeObserver(resize);
    resizeObserver.observe(gameContainer);
    resize();

    gameContainer.appendChild(app.canvas);

    try {
        const loadingThing = new LoadingThing(app);
        loadingThing.show();
        const assets = await loadAssets();
        loadingThing.hide();

        const game = new Game(assets, app, SoundManagerInstance);
        SoundManagerInstance.playGameMusic();

        app.ticker.add((time) => {
            game.update(time);
        });
    } catch (e) {
        console.error('Errore durante il caricamento del gioco:', e);
    }
}

function resize(): void {
    if (!app || !gameContainer) return;

    const w = gameContainer.clientWidth;
    const h = gameContainer.clientHeight;
    if (w === 0 || h === 0) return;

    const scale = Math.min(w / GAME_W, h / GAME_H);
    app.canvas.style.width  = Math.floor(GAME_W * scale) + 'px';
    app.canvas.style.height = Math.floor(GAME_H * scale) + 'px';
}

export function destroyGame(): void {
    resizeObserver?.disconnect();
    resizeObserver = null;
    try { (screen.orientation as unknown as { unlock: () => void }).unlock(); } catch { /* non supportato su tutti i browser */ }
    app?.destroy(true, { children: true });
    app = null;
    gameContainer = null;
}

import { Container } from 'pixi.js';
import { RellowAssets } from '../assets/RellowAssets';
import { ColorButton } from './ColorButton';
import { GAME_COLORS, shuffleArray } from './colors';

const BUTTON_SIZE  = 250;
const BUTTON_PADDING = 100;
const GRID_START_X = 60;
const GRID_START_Y = 720;
const STEP         = BUTTON_SIZE + BUTTON_PADDING; // 350
const TOTAL_BUTTONS  = 9;
const INITIAL_ACTIVE = 3;

export type RoundSetup = {
    wordText: string;
    wordForegroundColor: number;
};

export class ColorButtonGrid {
    onWon: (() => void) | null = null;
    onLost: (() => void) | null = null;

    private readonly _buttons: ColorButton[];
    private _activeCount: number = INITIAL_ACTIVE;
    private _currentWord: string = '';

    constructor(parent: Container, assets: RellowAssets) {
        this._buttons = [];

        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                const x = GRID_START_X + col * STEP;
                const y = GRID_START_Y + row * STEP;
                const color = GAME_COLORS[row * 3 + col]!;
                const btn = new ColorButton(parent, x, y, BUTTON_SIZE, assets, color);
                btn.onPressed = this._onButtonPressed;
                this._buttons.push(btn);
            }
        }

        this._applyVisibility([0, 1, 2]);
    }

    setupNewRound(): RoundSetup {
        const shuffledIndices = shuffleArray([0, 1, 2, 3, 4, 5, 6, 7, 8]);
        const activeIndices = shuffledIndices.slice(0, this._activeCount);

        this._applyVisibility(activeIndices);

        const foregroundIndex = activeIndices[Math.floor(Math.random() * activeIndices.length)]!;
        const wordNameIndex   = activeIndices[Math.floor(Math.random() * activeIndices.length)]!;

        const wordForegroundColor = GAME_COLORS[foregroundIndex]!.pixiColor;
        const wordText = GAME_COLORS[wordNameIndex]!.name;

        this._currentWord = wordText;

        return { wordText, wordForegroundColor };
    }

    addButton(): void {
        this._activeCount = Math.min(TOTAL_BUTTONS, this._activeCount + 1);
    }

    get canAddButtons(): boolean {
        return this._activeCount < TOTAL_BUTTONS;
    }

    get activeCount(): number {
        return this._activeCount;
    }

    update(deltaMS: number): void {
        for (const btn of this._buttons) {
            btn.update(deltaMS);
        }
    }

    destroy(): void {
        for (const btn of this._buttons) {
            btn.destroy();
        }
    }

    private _applyVisibility(activeIndices: number[]): void {
        const activeSet = new Set(activeIndices);
        this._buttons.forEach((btn, i) => {
            btn.visible = activeSet.has(i);
        });
    }

    private _onButtonPressed = (button: ColorButton): void => {
        if (button.color.name === this._currentWord) {
            this.onWon?.();
        } else {
            this.onLost?.();
        }
    };
}

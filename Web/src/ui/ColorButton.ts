import { Container, Sprite } from 'pixi.js';
import { RellowAssets } from '../assets/RellowAssets';
import { GameColor } from './colors';

const AUTO_RELEASE_MS = 150;

export class ColorButton {
    readonly color: GameColor;
    onPressed: ((button: ColorButton) => void) | null = null;

    private readonly _container: Container;
    private readonly _spriteUp: Sprite;
    private readonly _assets: RellowAssets;
    private _autoReleaseTimer: number = 0;

    constructor(
        parent: Container,
        x: number,
        y: number,
        size: number,
        assets: RellowAssets,
        color: GameColor,
    ) {
        this.color = color;
        this._assets = assets;

        this._container = new Container();
        this._container.position.set(x, y);

        const spriteBottom = new Sprite(assets.sprites.buttonBottom);
        spriteBottom.width = size;
        spriteBottom.height = size;

        this._spriteUp = new Sprite(assets.sprites.buttonUpReleased);
        this._spriteUp.tint = color.pixiColor;
        this._spriteUp.width = size;
        this._spriteUp.height = size;

        this._container.addChild(spriteBottom);
        this._container.addChild(this._spriteUp);

        this._container.eventMode = 'static';
        this._container.cursor = 'pointer';
        this._container.on('pointerdown', this._onPointerDown);

        parent.addChild(this._container);
    }

    get visible(): boolean {
        return this._container.visible;
    }

    set visible(v: boolean) {
        this._container.visible = v;
    }

    update(deltaMS: number): void {
        if (this._autoReleaseTimer > 0) {
            this._autoReleaseTimer -= deltaMS;
            if (this._autoReleaseTimer <= 0) {
                this._release();
            }
        }
    }

    destroy(): void {
        this._container.off('pointerdown', this._onPointerDown);
        this._container.destroy({ children: true });
    }

    private _onPointerDown = (): void => {
        if (!this._container.visible) return;
        this._spriteUp.texture = this._assets.sprites.buttonUpPressed;
        this._autoReleaseTimer = AUTO_RELEASE_MS;
        this.onPressed?.(this);
    };

    private _release(): void {
        this._autoReleaseTimer = 0;
        this._spriteUp.texture = this._assets.sprites.buttonUpReleased;
    }
}

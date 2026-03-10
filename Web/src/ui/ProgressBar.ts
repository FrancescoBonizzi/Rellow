import { Container, Graphics } from 'pixi.js';

const enum BarDirection { Up = 0, Right = 1, Down = 2, Left = 3 }

export class ProgressBar {
    private readonly _bar: Graphics;
    private _direction: BarDirection = BarDirection.Up;
    private _color: number;
    private _maxValue: number;
    private _ratio: number = 1;

    constructor(parent: Container, maxValue: number, color: number) {
        this._maxValue = maxValue;
        this._color = color;
        this._bar = new Graphics();
        parent.addChild(this._bar);
        this._redraw();
    }

    setMaxValue(max: number): void {
        this._maxValue = max;
    }

    setColor(color: number): void {
        this._color = color;
        this._redraw();
    }

    setValue(remaining: number): void {
        this._ratio = this._maxValue > 0 ? remaining / this._maxValue : 0;
        this._redraw();
    }

    reset(): void {
        this._direction = ((this._direction + 1) % 4) as BarDirection;
        this._ratio = 1;
        this._redraw();
    }

    private _redraw(): void {
        this._bar.clear();
        const r = this._ratio;
        switch (this._direction) {
            case BarDirection.Up:
                this._bar.rect(0, 0, 1080, 1920 * r).fill(this._color);
                break;
            case BarDirection.Right:
                this._bar.rect(1080 * (1 - r), 0, 1080 * r, 1920).fill(this._color);
                break;
            case BarDirection.Down:
                this._bar.rect(0, 1920 * (1 - r), 1080, 1920 * r).fill(this._color);
                break;
            case BarDirection.Left:
                this._bar.rect(0, 0, 1080 * r, 1920).fill(this._color);
                break;
        }
    }

    destroy(): void {
        this._bar.destroy();
    }
}

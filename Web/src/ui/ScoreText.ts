import { Container, Text, TextStyle } from 'pixi.js';
import { RellowAssets } from '../assets/RellowAssets';

const SCORE_X = 540;
const SCORE_Y = 167;
const POPUP_X = 540;
const POPUP_Y_START = 397;
const POPUP_DURATION = 2000;

export class ScoreText {
    private readonly _text: Text;
    private readonly _popup: Text;
    private _popupTimer: number = 0;

    constructor(parent: Container, assets: RellowAssets) {
        const style = new TextStyle({
            fontFamily: assets.fontName,
            fontSize: 80,
            fill: 0xffea87,
            align: 'center',
            stroke: { color: 0x000000, width: 6 },
        });

        this._text = new Text({ text: '0', style });
        this._text.anchor.set(0.5, 0.5);
        this._text.x = SCORE_X;
        this._text.y = SCORE_Y;
        parent.addChild(this._text);

        const popupStyle = new TextStyle({
            fontFamily: assets.fontName,
            fontSize: 60,
            fill: 0xffea87,
            align: 'center',
            stroke: { color: 0x000000, width: 4 },
        });

        this._popup = new Text({ text: '', style: popupStyle });
        this._popup.anchor.set(0.5, 0.5);
        this._popup.x = POPUP_X;
        this._popup.y = POPUP_Y_START;
        this._popup.visible = false;
        parent.addChild(this._popup);
    }

    setScore(score: number): void {
        this._text.text = `${score}`;
    }

    setColor(color: number): void {
        this._text.style.fill = color;
    }

    showPopup(diff: number, color: number): void {
        this._popup.text = `+${diff}`;
        this._popup.style.fill = color;
        this._popup.y = POPUP_Y_START;
        this._popup.alpha = 1;
        this._popup.visible = true;
        this._popupTimer = POPUP_DURATION;
    }

    update(deltaMS: number): void {
        if (this._popupTimer > 0) {
            this._popupTimer -= deltaMS;
            this._popup.y -= 0.1 * deltaMS;
            this._popup.alpha = Math.max(0, this._popupTimer / POPUP_DURATION);
            if (this._popupTimer <= 0) this._popup.visible = false;
        }
    }

    destroy(): void {
        this._text.destroy();
        this._popup.destroy();
    }
}

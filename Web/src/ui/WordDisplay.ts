import { Container, Graphics, Text } from 'pixi.js';
import { RellowAssets } from '../assets/RellowAssets';

const BG_X      = 0;
const BG_Y      = 500;
const BG_WIDTH  = 1080;
const BG_HEIGHT = 120;
const TEXT_X    = 540;
const TEXT_Y    = 560; // BG_Y + BG_HEIGHT / 2

export class WordDisplay {
    private readonly _container: Container;
    private readonly _text: Text;

    constructor(parent: Container, assets: RellowAssets) {
        this._container = new Container();

        const background = new Graphics();
        background.rect(BG_X, BG_Y, BG_WIDTH, BG_HEIGHT).fill(0x000000);
        this._container.addChild(background);

        this._text = new Text({
            text: '',
            style: {
                fontFamily: assets.fontName,
                fontSize: 80,
                fill: 0xffffff,
                align: 'center',
                stroke: { color: 0x000000, width: 4 },
            },
        });
        this._text.anchor.set(0.5, 0.5);
        this._text.position.set(TEXT_X, TEXT_Y);
        this._container.addChild(this._text);

        parent.addChild(this._container);
    }

    setWord(text: string, foregroundColor: number): void {
        this._text.text = text;
        this._text.style.fill = foregroundColor;
    }

    destroy(): void {
        this._container.destroy({ children: true });
    }
}

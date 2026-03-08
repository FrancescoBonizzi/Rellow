import { Assets, Spritesheet, Texture } from "pixi.js";
import { RellowAssets } from "./RellowAssets";

const VITE_BASE = import.meta.env.BASE_URL;
export const AssetsRoot: string = `${VITE_BASE}assets`;
const _spritesRoot = `${AssetsRoot}/sprites`;
const _fontsRoot = `${AssetsRoot}/fonts`;

export const loadAssets = async (): Promise<RellowAssets> => {
    const sheet = await Assets.load(`${_spritesRoot}/spritesheet.json`) as Spritesheet;
    const fontName = await loadFont();
    return {
        fontName,
        sprites: {
            buttonBottom: loadTexture(sheet, "button-bottom"),
            buttonUpPressed: loadTexture(sheet, "button-up-pressed"),
            buttonUpReleased: loadTexture(sheet, "button-up-released"),
            manina: loadTexture(sheet, "manina"),
        },
    };
};

const loadTexture = (sheet: Spritesheet, name: string): Texture =>
    sheet.textures[name]!;

const loadFont = async (): Promise<string> => {
    const fontFamily = "Press Start 2P";
    const font = new FontFace(fontFamily, `url(${_fontsRoot}/PressStart2P-Regular.woff2)`);
    await font.load();
    document.fonts.add(font);
    return fontFamily;
};

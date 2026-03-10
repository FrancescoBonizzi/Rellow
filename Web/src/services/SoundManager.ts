import { Howl } from "howler";
import { AssetsRoot } from "../assets/AssetsLoader";

class SoundManager {
    private readonly _sounds: Record<string, Howl> = {};

    private readonly _paths = {
        musicMenu: `${AssetsRoot}/sounds/music-menu.mp3`,
        musicPlay: `${AssetsRoot}/sounds/music-play.mp3`,
        effectOk: `${AssetsRoot}/sounds/effect-ok.mp3`,
        effectWrong: `${AssetsRoot}/sounds/effect-wrong.mp3`,
    };

    constructor() {
        this._sounds["music-menu"] = new Howl({ src: [this._paths.musicMenu], loop: true, volume: 0.7 });
        this._sounds["music-play"] = new Howl({ src: [this._paths.musicPlay], loop: true, volume: 0.9 });
        this._sounds["effect-ok"] = new Howl({ src: [this._paths.effectOk] });
        this._sounds["effect-wrong"] = new Howl({ src: [this._paths.effectWrong] });
    }

    playMenuMusic(): void { this._stopMusic(); this._sounds["music-menu"]!.play(); }
    playGameMusic(): void { this._stopMusic(); this._sounds["music-play"]!.play(); }
    playOk(): void { this._sounds["effect-ok"]!.play(); }
    playWrong(): void { this._sounds["effect-wrong"]!.play(); }
    stopAll(): void { Object.values(this._sounds).forEach(s => s.stop()); }

    private _stopMusic(): void {
        this._sounds["music-menu"]!.stop();
        this._sounds["music-play"]!.stop();
    }
}

export default SoundManager;

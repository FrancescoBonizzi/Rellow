export type GameColor = {
    readonly name: string;
    readonly pixiColor: number;
};

export type UIColorConfig = {
    timerBarColor: number;
    scoreTextColor: number;
    backgroundColor: number;
};

export const GAME_COLORS: GameColor[] = [
    { name: 'Giallo',    pixiColor: 0xffff00 },
    { name: 'Rosso',     pixiColor: 0xff0000 },
    { name: 'Blu',       pixiColor: 0x0000ff },
    { name: 'Verde',     pixiColor: 0x00ff00 },
    { name: 'Arancione', pixiColor: 0xff7300 },
    { name: 'Viola',     pixiColor: 0xff00ff },
    { name: 'Grigio',    pixiColor: 0x969696 },
    { name: 'Bianco',    pixiColor: 0xffffff },
    { name: 'Azzurro',   pixiColor: 0x00ffff },
];

export function shuffleArray<T>(arr: T[]): T[] {
    const copy = [...arr];
    for (let i = copy.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [copy[i], copy[j]] = [copy[j]!, copy[i]!];
    }
    return copy;
}

export function pickRandom<T>(arr: T[]): T {
    return arr[Math.floor(Math.random() * arr.length)]!;
}

export function shuffleUIColors(colors: GameColor[]): UIColorConfig {
    return {
        timerBarColor:   pickRandom(colors).pixiColor,
        scoreTextColor:  pickRandom(colors).pixiColor,
        backgroundColor: pickRandom(colors).pixiColor,
    };
}

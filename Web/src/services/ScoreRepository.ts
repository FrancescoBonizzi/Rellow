const ScorePrefix = 'rellow-score';
export type ScoreSource = 'gameover' | 'record';

const makeKey = (source: ScoreSource): string => `${ScorePrefix}-${source}`;

const toNumberOr0 = (value: string | null): number => {
    const n = Number(value);
    return isNaN(n) ? 0 : n;
};

export default {
    getScore: (source: ScoreSource): number =>
        toNumberOr0(localStorage.getItem(makeKey(source))),

    setScore: (source: ScoreSource, value: number): void => {
        localStorage.setItem(makeKey(source), value.toString());
    },

    isNewRecord: (score: number): boolean =>
        score > toNumberOr0(localStorage.getItem(makeKey('record'))),
};

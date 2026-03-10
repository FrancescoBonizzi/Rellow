const randomBetween = (min: number, max: number): number => {
    if (min === max) return min;
    if (min > max) [min, max] = [max, min];
    return min + Math.random() * (max - min);
};

export default {
    randomBetween,

    headOrTail: (): boolean => Math.random() > 0.5,

    toRadians: (degrees: number): number => degrees * Math.PI / 180,

    lerp: (start: number, end: number, t: number): number => start + t * (end - start),

    clamp01: (n: number): number => Math.max(0, Math.min(1, n)),

    clamp: (n: number, min: number, max: number): number => Math.max(min, Math.min(max, n)),

    easeOutCubic: (t: number): number => 1 - Math.pow(1 - t, 3),

    easeInCubic: (t: number): number => t * t * t,
};

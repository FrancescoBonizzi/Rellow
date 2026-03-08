# PATTERN COOKBOOK — C# → TypeScript/PixiJS

Pattern ricorrenti estratti dal progetto riferimento (InfartGame Web).
Ogni pattern mostra il codice C# originale e l'equivalente TypeScript da seguire.

---

## P01 — Bootstrap e inizializzazione app

**C# (RellowBootstrap.cs)**
```csharp
public static void Initialize(Game game) {
    game.Window.AllowUserResizing = false;
    game.IsMouseVisible = false;
    // ... setup
}
```

**TypeScript (gamebootstrap.ts)**
```typescript
const GAME_W = 1080;
const GAME_H = 1920;

let app: Application | null = null;
let resizeObserver: ResizeObserver | null = null;

export const initGame = async (container: HTMLElement) => {
    app = new Application();
    await app.init({
        width: GAME_W,
        height: GAME_H,
        autoDensity: true,
        resolution: Math.min(window.devicePixelRatio || 1, 2),
        antialias: true,
        background: 0xff1243,  // PrimaryBackgroundColor
    });

    container.appendChild(app.canvas);
    resizeObserver = new ResizeObserver(resize);
    resizeObserver.observe(container);
    resize();

    try { await screen.orientation.lock('portrait'); } catch {}

    const assets = await loadAssets();
    const game = new Game(app, assets, SoundManager.instance);
    app.ticker.add((t) => game.update(t));
};

export const destroyGame = () => {
    app?.destroy(true);
    app = null;
    resizeObserver?.disconnect();
    resizeObserver = null;
    try { screen.orientation.unlock(); } catch {}
};

function resize() {
    if (!app || !container) return;
    const containerW = container.clientWidth;
    const containerH = container.clientHeight;
    if (containerW === 0 || containerH === 0) return;

    const scale = Math.min(containerW / GAME_W, containerH / GAME_H);
    app.renderer.resize(GAME_W, GAME_H);
    app.canvas.style.width  = Math.floor(GAME_W * scale) + 'px';
    app.canvas.style.height = Math.floor(GAME_H * scale) + 'px';
}
```

> **Regola**: sempre `autoDensity: true` + `resolution: Math.min(DPR, 2)`.
> Resize tramite CSS, non ridimensionando il renderer. Vedi `RESPONSIVE_GAME_PATTERN.md`.

---

## P02 — Game loop (Update)

**C# (RellowGame.cs)**
```csharp
public void Update(GameTime gameTime) {
    _timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
    _progressBar.Update(gameTime);
    _buttonManager.Update(gameTime);
    if (_timer <= 0) OnLost();
}
```

**TypeScript (Game.ts)**
```typescript
update(time: Ticker) {
    if (this._state === 'waiting_for_input') {
        this._timer -= time.deltaMS;
        this._progressBar.update(time);
        if (this._timer <= 0) this.onLost();
    }
}
```

> `time.deltaMS` = millisecondi trascorsi dall'ultimo frame (equivalente di `ElapsedGameTime.TotalMilliseconds`).
> `time.deltaTime` = ratio rispetto a 60fps (1.0 = frame normale, 2.0 = frame saltato).
> Per timer in ms, usare sempre **`deltaMS`**.

---

## P03 — Router-based state machine

**C# (GameOrchestrator.cs)**
```csharp
private GameStates _currentState;
public void TransitionTo(GameStates newState) {
    _fade.StartFadeOut(() => {
        _currentState = newState;
        _fade.StartFadeIn();
    });
}
```

**TypeScript (router.ts)**
```typescript
import Navigo from 'navigo';
export const router = new Navigo('/');

const appElement = document.getElementById('app')!;

router
    .on('/', () => renderMenuPage(appElement))
    .on('/game', () => initGame(document.getElementById('game-container')!), {
        leave: (done) => {
            import('./gamebootstrap').then(m => {
                m.destroyGame?.();
                done();
            });
        }
    })
    .on('/gameover', () => renderGameOverPage(appElement));
```

> Non c'è più uno stato enum: le routes Navigo SONO gli stati.
> Il cleanup della route precedente avviene nel hook `leave:`.

---

## P04 — Asset loading tipizzato

**C# (AssetsLoader.cs)**
```csharp
public class AssetsLoader {
    public SpriteFont TitleFont { get; private set; }
    public IDictionary<string, Sprite> Sprites { get; }

    public void LoadResources() {
        TitleFont = _contentManager.Load<SpriteFont>("font-title");
        Sprites["button-bottom"] = ...;
    }
}
```

**TypeScript (AssetsLoader.ts + RellowAssets.ts)**
```typescript
// RellowAssets.ts — interface tipizzata
export interface RellowAssets {
    fontName: string;
    sprites: {
        buttonBottom: Texture;
        buttonUpPressed: Texture;
        buttonUpReleased: Texture;
    };
}

// AssetsLoader.ts — funzione async
export const loadAssets = async (): Promise<RellowAssets> => {
    const sheet = await Assets.load('/assets/sprites/spritesheet.json');
    const fontName = await loadFont('/assets/fonts/font-title.woff2', 'RellowFont');

    return {
        fontName,
        sprites: {
            buttonBottom:      sheet.textures['button-bottom'],
            buttonUpPressed:   sheet.textures['button-up-pressed'],
            buttonUpReleased:  sheet.textures['button-up-released'],
        }
    };
};

async function loadFont(url: string, family: string): Promise<string> {
    const font = new FontFace(family, `url(${url})`);
    await font.load();
    document.fonts.add(font);
    return family;
}
```

---

## P05 — Audio management con Howler

**C# (SoundManager.cs)**
```csharp
public class SoundManager {
    private readonly Dictionary<string, SoundEffectInstance> _sounds = new();

    public void PlayMenu() {
        StopSounds();
        _sounds["music-menu"].IsLooped = true;
        _sounds["music-menu"].Play();
    }

    public void PlayWin() => _sounds["effect-win"].Play();
}
```

**TypeScript (SoundManager.ts)**
```typescript
export class SoundManager {
    private static _instance: SoundManager;
    static get instance() { return this._instance ??= new SoundManager(); }

    private readonly _sounds: Record<string, Howl> = {};

    private constructor() {
        this._sounds['music-menu'] = new Howl({
            src: ['/assets/sounds/music-menu.mp3'],
            loop: true,
            volume: 0.7
        });
        this._sounds['music-playing'] = new Howl({
            src: ['/assets/sounds/music-play.mp3'],
            loop: true,
            volume: 0.9
        });
        this._sounds['effect-win'] = new Howl({
            src: ['/assets/sounds/effect-ok.mp3']
        });
        this._sounds['effect-loose'] = new Howl({
            src: ['/assets/sounds/effect-wrong.mp3']
        });
    }

    playMenu() {
        this.stopSounds();
        this._sounds['music-menu'].play();
    }

    playPlaying() {
        this.stopSounds();
        this._sounds['music-playing'].play();
    }

    playWin()   { this._sounds['effect-win'].play(); }
    playLoose() { this._sounds['effect-loose'].play(); }

    stopSounds() {
        Object.values(this._sounds).forEach(s => s.stop());
    }
}
```

> Nota: il browser blocca l'AudioContext finché non avviene una user gesture.
> Chiamare `wireAudioUnlockOnce()` in `main.ts`. Vedi `AUDIO_UNLOCK_PATTERN.md`.

---

## P06 — Sprite da spritesheet

**C# (con `CustomSpriteImporter`)**
```csharp
Sprites["button-bottom"] = _textureImporter.GetSprite("button-bottom");
// Il Rectangle viene letto dal Spritesheet.txt
```

**TypeScript**
```typescript
// Caricare il JSON atlas (TexturePacker formato PixiJS)
const sheet = await Assets.load('/assets/sprites/spritesheet.json');

// Accesso diretto alle texture per nome
const buttonBottomTexture: Texture = sheet.textures['button-bottom'];

// Creare uno Sprite
const sprite = new Sprite(buttonBottomTexture);
sprite.position.set(x, y);
container.addChild(sprite);
```

---

## P07 — Colore PixiJS

**C# (XNA Color)**
```csharp
var color = new Color(255, 18, 67);     // rgb
var white = Color.White;
var transparent = Color.Transparent;
// tint sprite:
spriteBatch.Draw(texture, rect, color);
```

**TypeScript (PixiJS)**
```typescript
// Come numero hex:
const color = 0xff1243;

// Come stringa:
const color = '#ff1243';

// Tint su Sprite:
sprite.tint = 0xff1243;
sprite.tint = 0xffffff; // reset tint (bianco = nessun effetto)

// Alpha:
sprite.alpha = 0.7;

// Colore su Text:
const text = new Text({ text: 'RELLOW', style: { fill: 0xffea87 } });

// Colore su Graphics:
const g = new Graphics();
g.rect(0, 0, 100, 100).fill(0xff1243);
```

> **Conversione rgb → hex**: `(r << 16) | (g << 8) | b`
> Es: `(255 << 16) | (18 << 8) | 67` = `0xff1243`

---

## P08 — Text con PixiJS

**C# (SpriteFont)**
```csharp
spriteBatch.DrawString(font, "RELLOW", position, Color.White, 0f, origin, scale, ...);
```

**TypeScript (PixiJS Text)**
```typescript
const text = new Text({
    text: 'RELLOW',
    style: {
        fontFamily: 'RellowFont',  // nome registrato via FontFace
        fontSize: 120,
        fill: 0xffea87,
        align: 'center',
    }
});
text.anchor.set(0.5);  // pivot al centro (equivalente di DrawString origin)
text.position.set(540, 200);  // coordinate in spazio 1080×1920
container.addChild(text);
```

---

## P09 — ProgressBar (timer visivo)

**C# (ProgressBar.cs)**
```csharp
// 4 direzioni, barra che si svuota
_progressBar.CurrentValue = _timer;
_progressBar.Update(gameTime);
_progressBar.Draw(spriteBatch);
```

**TypeScript (ProgressBar.ts)**
```typescript
export class ProgressBar {
    private readonly _bar: Graphics;
    private readonly _maxValue: number;
    private _currentValue: number;

    constructor(parent: Container, maxValue: number, color: number) {
        this._bar = new Graphics();
        parent.addChild(this._bar);
        this._maxValue = maxValue;
        this._currentValue = maxValue;
    }

    setValue(value: number) {
        this._currentValue = Math.max(0, value);
        this._redraw();
    }

    private _redraw() {
        const ratio = this._currentValue / this._maxValue;
        this._bar.clear();
        this._bar.rect(0, 0, 1080 * ratio, 20).fill(this._color);
    }
}
```

---

## P10 — Persistence con localStorage

**C# (ISettingsRepository)**
```csharp
_settingsRepository.SetInt("SCORE", currentScore);
int best = _settingsRepository.GetOrSetInt("SCORE", 0);
```

**TypeScript (ScoreRepository.ts)**
```typescript
export const ScoreRepository = {
    getScore(): number {
        return parseInt(localStorage.getItem('SCORE') ?? '0', 10) || 0;
    },
    setScore(value: number): void {
        localStorage.setItem('SCORE', value.toString());
    }
};
```

---

## P11 — Nomi colori (solo italiano, nessuna localizzazione)

I nomi dei colori sono **hardcoded in italiano**. Non serve alcun sistema di localizzazione.

**TypeScript (colors.ts)**
```typescript
export type GameColor = {
    name: string;
    pixiColor: number;
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
```

---

## P12 — Input su bottone PixiJS (click/touch)

**C# (GameButton.cs)**
```csharp
// Polling ogni frame
if (_inputHandler.IsPressed && CollisionRect.Contains(_inputHandler.Position)) {
    OnPressed();
}
```

**TypeScript — approccio A (eventMode su sprite)**
```typescript
const button = new Sprite(texture);
button.eventMode = 'static';
button.cursor = 'pointer';
button.on('pointerdown', () => onPressed());
container.addChild(button);
```

**TypeScript — approccio B (hit test manuale nel game loop)**
```typescript
// Nel Controller:
class Controller {
    private _lastPointerX = 0;
    private _lastPointerY = 0;
    private _pressed = false;

    constructor() {
        window.addEventListener('pointerdown', (e) => {
            this._lastPointerX = e.clientX;
            this._lastPointerY = e.clientY;
            this._pressed = true;
        });
        window.addEventListener('pointerup', () => { this._pressed = false; });
    }

    consumePress(): { x: number; y: number } | null {
        if (this._pressed) {
            this._pressed = false;
            return { x: this._lastPointerX, y: this._lastPointerY };
        }
        return null;
    }
}

// Nel Game.update():
const press = this._controller.consumePress();
if (press) {
    const gamePoint = app.renderer.events.mapPositionToPoint(
        new Point(), press.x, press.y
    );
    // check quale bottone è stato premuto
}
```

> **Raccomandato per Rellow**: Approccio A (eventMode = 'static') perché l'input è sempre
> diretto su bottoni visibili. Approccio B va bene per giochi con area di click generica.

---

## P13 — Animazione floating text (scaling sinusoidale)

**C# (FloatingText.cs)**
```csharp
private float _scale = 1.0f;
private float _scaleDirection = 1f;
private const float MinScale = 1.0f, MaxScale = 1.2f;

public void Update(GameTime gameTime) {
    _scale += _scaleDirection * 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
    if (_scale >= MaxScale) { _scale = MaxScale; _scaleDirection = -1f; }
    if (_scale <= MinScale) { _scale = MinScale; _scaleDirection = 1f; }
    // applica scale al draw
}
```

**TypeScript**
```typescript
private _scaleDir = 1;
private readonly _minScale = 1.0;
private readonly _maxScale = 1.2;

update(time: Ticker) {
    this._sprite.scale.x += this._scaleDir * 0.3 * (time.deltaMS / 1000);
    this._sprite.scale.y = this._sprite.scale.x;
    if (this._sprite.scale.x >= this._maxScale) {
        this._sprite.scale.x = this._maxScale;
        this._scaleDir = -1;
    }
    if (this._sprite.scale.x <= this._minScale) {
        this._sprite.scale.x = this._minScale;
        this._scaleDir = 1;
    }
}
```

---

## P14 — Griglia bottoni (GameButtonsManager)

**C# (GameButtonsManager.cs)**
```csharp
// Griglia 3x3, 250x250px, padding 100px, start (60, 720)
for (int row = 0; row < 3; row++) {
    for (int col = 0; col < 3; col++) {
        var pos = new Vector2(60 + col * 350, 720 + row * 350);
        _buttons.Add(new GameButton(pos, _assets));
    }
}
// activeCount parte da 3, aumenta fino a 9
```

**TypeScript (ColorButtonGrid.ts)**
```typescript
const BUTTON_SIZE = 250;
const BUTTON_PADDING = 100;
const GRID_START_X = 60;
const GRID_START_Y = 720;
const STEP = BUTTON_SIZE + BUTTON_PADDING; // 350

export class ColorButtonGrid {
    private readonly _buttons: ColorButton[] = [];
    private _activeCount = 3;

    constructor(parent: Container, assets: RellowAssets) {
        for (let row = 0; row < 3; row++) {
            for (let col = 0; col < 3; col++) {
                const x = GRID_START_X + col * STEP;
                const y = GRID_START_Y + row * STEP;
                const btn = new ColorButton(parent, x, y, BUTTON_SIZE, assets);
                this._buttons.push(btn);
            }
        }
        this._updateVisibility();
    }

    setActiveCount(n: number) {
        this._activeCount = Math.min(9, Math.max(1, n));
        this._updateVisibility();
    }

    private _updateVisibility() {
        this._buttons.forEach((btn, i) => {
            btn.visible = i < this._activeCount;
        });
    }
}
```

---

## P15 — Shuffle colori UI (meccanica distrazione)

**C# (UIColors.cs + ColorWithName.cs)**
```csharp
// Ogni round, i colori della UI vengono rimescolati casualmente
public static void ShuffleUIColors() {
    TimerBarColor = _allColors[Random.Next(_allColors.Count)];
    ScoreTextColor = _allColors[Random.Next(_allColors.Count)];
    BackgroundColor = _allColors[Random.Next(_allColors.Count)];
    // CurrentWordBackground sempre nero
}

// Il nome del colore alterna casualmente tra primario e secondario
public void ShuffleColorName() {
    _showSecondaryName = Random.Shared.NextDouble() < 0.5;
}
```

**TypeScript**
```typescript
function pickRandom<T>(arr: T[]): T {
    return arr[Math.floor(Math.random() * arr.length)];
}

function shuffleUIColors(colors: GameColor[]): UIColorConfig {
    return {
        timerBarColor:   pickRandom(colors).pixiColor,
        scoreTextColor:  pickRandom(colors).pixiColor,
        backgroundColor: pickRandom(colors).pixiColor,
        wordBgColor:     0x000000,  // sempre nero
    };
}

function getColorDisplayName(color: GameColor): string {
    return color.name;  // sempre in italiano, nessuna variante
}
```

---

## P16 — Score popup animato

**C# (FloatingText con timeout)**
```csharp
private float _scorePopupTimer = 0;
public void ShowScorePopup(int points) {
    _scorePopupText.Text = $"+{points}";
    _scorePopupTimer = 2000;
}
public void Update(GameTime gt) {
    if (_scorePopupTimer > 0) {
        _scorePopupTimer -= gt.ElapsedGameTime.TotalMilliseconds;
        _popupSprite.Visible = true;
    } else {
        _popupSprite.Visible = false;
    }
}
```

**TypeScript**
```typescript
showPopup(points: number) {
    this._popupText.text = `+${points}`;
    this._popupText.visible = true;
    setTimeout(() => { this._popupText.visible = false; }, 2000);
}
```

> Nota: `setTimeout` è OK per animazioni UI one-shot. Per cose che interagiscono
> con la pausa del gioco, gestire il timer nel `update()` con `deltaMS`.

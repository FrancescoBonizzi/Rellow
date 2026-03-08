# ARCHITECTURE MAP — Rellow: MonoGame → PixiJS

## 1. Il gioco

Rellow è un gioco di reazione ai colori stile Stroop:
- Viene mostrato un nome di colore (es. "ROSSO") con un colore di testo che può essere diverso
- Il giocatore deve premere il bottone che corrisponde al **colore del testo**, non al nome scritto
- C'è un timer che scorre, ogni 5 vittorie aumenta la difficoltà
- Orientamento: **portrait (1080×1920)**

---

## 2. Stack tecnologico target

| Layer | Libreria |
|---|---|
| Rendering 2D | PixiJS 8.x |
| Audio | Howler.js 2.x |
| Router / State | Navigo 8.x |
| Bundler | Vite |
| Linguaggio | TypeScript 5.x |

Riferimento: `/Users/fbonizzi/Source/Rellow/progetto-riferimento/InfartGame/Web/`

---

## 3. Mapping classi principali

### 3.1 Bootstrap e lifecycle

| C# | TypeScript | Note |
|---|---|---|
| `RellowBootstrap.cs` | `src/main.ts` + `gamebootstrap.ts` | main inizializza router e audio unlock, gamebootstrap crea l'app PixiJS |
| `GameOrchestrator.cs` (enum `GameStates`) | `src/pages/router.ts` (Navigo routes) | `/` = menu, `/game` = gameplay |
| `RellowBootstrap.Initialize()` | `initGame()` in `gamebootstrap.ts` | Crea `Application`, carica asset, avvia ticker |
| `FadeObject` (transizioni 800ms) | CSS `transition`/`opacity` su page containers | Oppure PixiJS filter su stage alpha |
| `GameOrchestrator.StartRound()` → `RellowGame` | route `/game` → `initGame()` → `new Game(...)` | La route ha `leave:` hook che chiama `destroyGame()` |

### 3.2 Game loop

| C# | TypeScript | Note |
|---|---|---|
| `RellowGame.Update(GameTime)` | `game.update(time: Ticker)` | Chiamato da `app.ticker.add(...)` |
| `RellowGame.Draw(SpriteBatch)` | Implicito (scene graph PixiJS) | Non esiste un Draw separato |
| `GameTime.ElapsedGameTime.TotalSeconds` | `time.deltaTime` (in frame) o `time.deltaMS` (ms) | PixiJS Ticker: `deltaMS` = ms trascorsi, `deltaTime` = ratio a 60fps |
| Virtual resolution 1080×1920 | `const GAME_W = 1080; const GAME_H = 1920;` | Passato ad `app.init()` |
| `ScaleMatrix` per responsive | `resize()` con letterboxing CSS | Vedi `RESPONSIVE_GAME_PATTERN.md` |

### 3.3 Rendering e scene graph

| C# | TypeScript | Note |
|---|---|---|
| `SpriteBatch.Begin()` / `.Draw()` / `.End()` | Aggiungere figli a `Container` una volta | PixiJS renderizza automaticamente ogni frame |
| `RenderTarget2D` (1080×1920) | `app.renderer` (dimensionato a 1080×1920) | |
| `Texture2D` / `Rectangle` frame | `Sprite` / `AnimatedSprite` da spritesheet | |
| `SpriteFont` | `BitmapFont` o `Text` con font CSS `@font-face` | |
| Layer Z-order (draw order) | Ordine `addChild()` + `zIndex` | PixiJS ordina per posizione nell'array |
| `Color` (XNA struct) | `0xRRGGBB` number o `"#RRGGBB"` string | PixiJS accetta entrambe le forme |

### 3.4 Input

| C# | TypeScript | Note |
|---|---|---|
| `MouseListener` (click sinistro) | `pointerdown` event su Canvas | Unificato con touch |
| `TouchListener` | `pointerdown` event (stesso handler) | Il browser normalizza mouse e touch |
| `GamePadListener` (back button) | `keydown` con `e.key === "Escape"` | |
| Polling `Keyboard.GetState()` | Event listener + flag booleano | Vedi `Controller.ts` pattern |
| `Rectangle.Contains(point)` | `sprite.getBounds().containsPoint(x, y)` | Oppure `sprite.eventMode = 'static'` + onClick |

### 3.5 Audio

| C# | TypeScript | Note |
|---|---|---|
| `SoundManager.cs` | `SoundManager.ts` (Howler.js) | Stesso pattern, API diversa |
| `SoundEffectInstance.Play()` | `howl.play()` | |
| `SoundEffectInstance.Loop = true` | `new Howl({ loop: true })` | |
| `SoundEffectInstance.Volume = 0.7` | `new Howl({ volume: 0.7 })` | |
| `SoundManager.PauseAll()` | `Howler.mute(true)` oppure `.pause()` per ogni Howl | |
| Blocco browser autoplay | Pattern `wireAudioUnlockOnce()` | Vedi `AUDIO_UNLOCK_PATTERN.md` |

### 3.6 Asset loading

| C# | TypeScript | Note |
|---|---|---|
| `ContentManager.Load<Texture2D>("textures")` | `Assets.load(...)` (PixiJS) | Async/await |
| `Spritesheet.txt` (custom importer) | `atlas.json` (TexturePacker format) | JSON standard PixiJS |
| `ContentManager.Load<SpriteFont>(...)` | `Assets.load({ alias, src })` per bitmap font | Oppure `document.fonts.load()` per CSS font |
| `ContentManager.Load<SoundEffect>(...)` | Howler lazy-load o preload in constructor | |
| Tutto sincrono nel thread principale | Tutto `async/await` — mostrare loading screen | |

### 3.7 Persistence

| C# | TypeScript | Note |
|---|---|---|
| `ISettingsRepository.SetInt(key, value)` | `localStorage.setItem(key, value.toString())` | |
| `ISettingsRepository.GetOrSetInt(key, default)` | `parseInt(localStorage.getItem(key) ?? default)` | |
| Chiave `"SCORE"` | Stessa chiave — identica compatibilità | |

### 3.8 Localizzazione

| C# | TypeScript | Note |
|---|---|---|
| `GameStringsLoader` con dizionario `it`/`en` | Oggetto `translations` con chiavi lingua | |
| `ILocalizedStringsRepository.GetString(key)` | Funzione `t(key: string): string` | |
| `navigator.Language` per detect lingua | `navigator.language.startsWith('it')` | |

---

## 4. Struttura directory Web target

```
Web/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
├── public/
│   └── assets/
│       ├── sprites/          ← spritesheet PNG + JSON atlas
│       ├── sounds/           ← .mp3 per suoni e musiche
│       └── fonts/            ← font files
└── src/
    ├── main.ts               ← wireAudioUnlockOnce() + router.resolve()
    ├── Game.ts               ← game loop principale
    ├── assets/
    │   ├── AssetsLoader.ts   ← carica tutti gli asset, ritorna typed interface
    │   └── RellowAssets.ts   ← interface tipizzata degli asset
    ├── pages/
    │   ├── router.ts         ← Navigo: routes / e /game
    │   ├── menu.ts           ← render menu HTML + audio button
    │   └── gameover.ts       ← render game over con score
    ├── ui/
    │   ├── ColorButton.ts    ← bottone colorato con colore e nome
    │   ├── ColorButtonGrid.ts ← griglia NxN di bottoni
    │   ├── ProgressBar.ts    ← barra timer
    │   ├── ScoreText.ts      ← testo punteggio con popup
    │   └── WordDisplay.ts    ← display nome colore con colore testo variabile
    ├── services/
    │   ├── SoundManager.ts   ← Howler wrapper
    │   ├── SoundInstance.ts  ← singleton
    │   ├── AudioUnlocker.ts  ← wireAudioUnlockOnce, unlockHowler
    │   ├── ScoreRepository.ts ← localStorage wrapper
    │   ├── GameParameters.ts ← DynamicGameParameters (difficoltà)
    │   └── Localization.ts   ← t() function + color names
    ├── interaction/
    │   └── Controller.ts     ← input events (pointer/keyboard)
    └── uiKit/
        └── LoadingThing.ts   ← loading screen
```

---

## 5. Lifecycle completo

```
index.html caricato
    ↓
main.ts:
  ├─ wireAudioUnlockOnce()
  └─ router.resolve()
       ↓ route "/"
menu.ts:
  ├─ renderMenu(container)
  ├─ SoundManager.playMenu()
  └─ click "GIOCA" → router.navigate('/game')
       ↓
gamebootstrap.ts:
  ├─ app = new Application()
  ├─ await app.init({ width:1080, height:1920, ... })
  ├─ setup resize() + ResizeObserver
  ├─ screen.orientation.lock('portrait')
  ├─ assets = await loadAssets()
  ├─ game = new Game(app, assets, soundManager)
  └─ app.ticker.add(t => game.update(t))
       ↓ ogni frame
Game.update(time: Ticker):
  ├─ se waiting_for_input: aspetta click su bottone
  ├─ se bottone giusto → onWon()
  ├─ se bottone sbagliato → onLost()
  └─ se timer scaduto → onLost()
       ↓ game over
router.navigate('/gameover')
  ↓ leave hook chiama destroyGame()
gameover.ts:
  ├─ mostra score + record
  └─ click "GIOCA ANCORA" → router.navigate('/game')
```

---

## 6. Parametri di gioco

| Parametro | Valore iniziale | Variazione |
|---|---|---|
| Numero bottoni attivi | 3 | +1 ogni 5 vittorie (max 9) |
| Tempo per round | 2500ms | -200ms ogni 5 vittorie |
| Pausa tra round | 150ms | fisso |
| Formula punteggio | `(vittorie * 10) + (tempo_rimanente_ms / 10)` | — |
| Griglia bottoni | 3×3 (250×250px, padding 100px) | layout fisso, bottoni nascosti |
| Start pos griglia | (60, 720) in virtual coords 1080×1920 | — |

---

## 7. Struttura colori (9 colori)

I nomi visualizzati in game sono **solo in italiano**. Nessuna localizzazione.

```typescript
type GameColor = {
    rgb: { r: number; g: number; b: number };
    name: string;         // unico nome italiano visualizzato in game
    pixiColor: number;    // 0xRRGGBB
};

const COLORS: GameColor[] = [
    { rgb: {r:255,g:255,b:0},   name:"Giallo",    pixiColor: 0xffff00 },
    { rgb: {r:255,g:0,  b:0},   name:"Rosso",     pixiColor: 0xff0000 },
    { rgb: {r:0,  g:0,  b:255}, name:"Blu",       pixiColor: 0x0000ff },
    { rgb: {r:0,  g:255,b:0},   name:"Verde",     pixiColor: 0x00ff00 },
    { rgb: {r:255,g:115,b:0},   name:"Arancione", pixiColor: 0xff7300 },
    { rgb: {r:255,g:0,  b:255}, name:"Viola",     pixiColor: 0xff00ff },
    { rgb: {r:150,g:150,b:150}, name:"Grigio",    pixiColor: 0x969696 },
    { rgb: {r:255,g:255,b:255}, name:"Bianco",    pixiColor: 0xffffff },
    { rgb: {r:0,  g:255,b:255}, name:"Azzurro",   pixiColor: 0x00ffff },
];
```

---

## 8. Colori UI (palette di gioco)

| Costante | Valore | Uso |
|---|---|---|
| `PrimaryBackgroundColor` | `rgb(255, 18, 67)` | Background menu e game |
| `PrimaryForegroundColor` | `rgb(255, 234, 135)` | Testo titolo e UI primaria |
| UI color shuffle ogni round | colori casuali per: timer bar, score text, background | Distrazione visiva (meccanica di gioco!) |

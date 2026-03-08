# CLAUDE.md — Regole di migrazione Rellow: MonoGame → PixiJS

## Contesto

Stiamo migrando Rellow (gioco C#/MonoGame) a PixiJS/TypeScript.
- **Sorgente**: `/Users/fbonizzi/Source/Rellow/CSharp/`
- **Target**: `/Users/fbonizzi/Source/Rellow/Web/`
- **Riferimento completato**: `/Users/fbonizzi/Source/Rellow/progetto-riferimento/InfartGame/Web/`
- **Docs architettura**: `/Users/fbonizzi/Source/Rellow/migration-specs/`

---

## Regole NON negoziabili

### 1. Stack tecnologico

Usare **esattamente** questo stack (identico al progetto riferimento):
- `pixi.js` ^8.x
- `howler` ^2.x
- `navigo` ^8.x
- Vite come bundler
- TypeScript 5.x

Non introdurre altre dipendenze senza approvazione esplicita.

### 2. Responsive — orientamento portrait

Rellow è **sempre portrait (1080×1920)**. Seguire `RESPONSIVE_GAME_PATTERN.md` con queste eccezioni:
- `app.init()` con `autoDensity: true` e `resolution: Math.min(window.devicePixelRatio || 1, 2)`
- Resize tramite CSS (`canvas.style.width/height`), non ridimensionando il renderer
- `ResizeObserver` (non `window.addEventListener('resize')`)
- `100svh` (non `100vh`) nell'HTML/CSS
- `screen.orientation.lock('portrait')` in `try/catch`
- **NO overlay "Ruota il dispositivo"**: il gioco è portrait-only, non serve gestire il landscape

### 3. Audio unlock

Seguire in modo preciso `AUDIO_UNLOCK_PATTERN.md`:
- `wireAudioUnlockOnce()` chiamato in `main.ts` prima di qualsiasi altro codice
- Bottone esplicito "Attiva audio" nel menu (non solo sblocco automatico)
- Non chiamare `audioContext.resume()` dentro `setTimeout` — deve essere sincrono rispetto alla gesture

### 4. Riusa il codice del progetto riferimento — non reinventare

Il progetto riferimento `/Users/fbonizzi/Source/Rellow/progetto-riferimento/InfartGame/Web/src/` è una base già funzionante e collaudata. **Prima di scrivere qualsiasi file, controllare se esiste già nel riferimento e copiarlo adattandolo.**

File da copiare quasi pari pari (cambiando solo nomi/costanti specifici di Rellow):
- `main.ts` — bootstrap, audio unlock, router resolve
- `pages/router.ts` — struttura Navigo con leave hook
- `services/AudioUnlocker.ts` — copia identica
- `services/SoundManager.ts` — stessa struttura, cambiare solo i file audio
- `services/ScoreRepository.ts` — stessa struttura, cambiare le chiavi
- `services/Numbers.ts` — copia identica (utility math)
- `uiKit/LoadingThing.ts` — copia identica
- `gamebootstrap.ts` — stessa struttura, adattare dimensioni (1080×1920) e nome Game class
- `Game.ts` — stessa struttura del loop (ticker, update, destroy), adattare la logica interna
- `assets/AssetsLoader.ts` — stesso pattern async/typed, cambiare gli asset caricati

L'obiettivo è **continuità di stile e architettura** tra i giochi di questo autore. Se il riferimento fa una cosa in un certo modo, Rellow la fa nello stesso modo — salvo differenze di gameplay documentate in questo file.

### 5. Pattern da seguire

Tutti i pattern sono in `migration-specs/PATTERN_COOKBOOK.md`. Prima di scrivere codice, verificare se esiste un pattern applicabile.

### 6. Design delle pagine HTML/CSS

Le pagine menu e game over sono HTML/CSS puro (fuori dal canvas PixiJS). Devono essere:

- **Alta qualità visiva**: il gioco è colorato, vivace, in stile pixel art. Le pagine HTML devono rispecchiare questo carattere — non essere generiche o minimal.
- **Coerenza con la palette**: usare `#ff1243` (primario), `#ffea87` (testo/accenti), nero per contrasti.
- **Font pixel art**: usare un font monospace o pixel-style che evochi l'estetica del gioco (es. "Press Start 2P", "VT323", o il font di gioco stesso se compatibile con CSS).
- **Responsive rigoroso**:
  - Layout fluido con `clamp()` per font size e spaziature
  - Bottoni con area di tap adeguata su mobile (min 48×48px area touch)
  - Testare sia su schermo piccolo (320px) che grande (tablet portrait)
  - Nessun overflow orizzontale
- **Atmosfera**: animazioni CSS leggere (pulse, bounce, glow) per dare vita ai bottoni e al titolo — coerenti con l'energia del gioco.

Non usare framework CSS (Bootstrap, Tailwind). CSS vanilla o CSS modules.

### 7. Nessuna modifica al C# originale

Non toccare i file in `/CSharp/`. Sono solo sorgente di riferimento.

### 8. TypeScript strict

Niente `any` implicito. Tipizzare sempre gli asset con interface dedicata (`RellowAssets`).

---

## Struttura directory Web

```
Web/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
├── public/assets/
│   ├── sprites/     ← spritesheet.json + spritesheet.png (TexturePacker formato PixiJS)
│   ├── sounds/      ← .mp3
│   └── fonts/       ← .woff2
└── src/
    ├── main.ts
    ├── Game.ts
    ├── assets/AssetsLoader.ts + RellowAssets.ts
    ├── pages/router.ts + menu.ts + gameover.ts
    ├── ui/ColorButton.ts + ColorButtonGrid.ts + ProgressBar.ts + WordDisplay.ts + ScoreText.ts
    ├── services/SoundManager.ts + SoundInstance.ts + AudioUnlocker.ts + ScoreRepository.ts + Localization.ts + GameParameters.ts
    └── interaction/Controller.ts
```

---

## Meccanica di gioco (non dimenticare!)

1. **Stroop effect**: viene mostrato un nome colore (es. "ROSSO") con un colore del testo **diverso** dal nome. Il giocatore deve premere il bottone del colore **del testo**, non del nome scritto.

2. **Griglia 3×3**: 9 bottoni, inizialmente solo 3 attivi. Ogni 5 vittorie si aggiunge 1 bottone fino a 9.

3. **Timer decrescente**: inizia a 2500ms, ogni 5 vittorie si riduce di 200ms.

4. **Shuffle colori UI ogni round**: timer bar, score text e background cambiano colore casualmente — è parte della meccanica di distrazione.

5. **Nome colore**: sempre il nome italiano, nessuna variante secondaria, nessuna localizzazione.

6. **Pausa 150ms** tra un round e il successivo (dopo una vittoria).

7. **Formula punteggio**: `(vittorie * 10) + (tempo_rimanente_ms / 10)`

8. **9 colori** (nomi solo in italiano, hardcoded, nessuna localizzazione): Giallo, Rosso, Blu, Verde, Arancione, Viola, Grigio, Bianco, Azzurro

---

## Colori costanti

| Nome | Hex | Uso |
|---|---|---|
| `PrimaryBackground` | `#ff1243` | Background menu e gioco |
| `PrimaryForeground` | `#ffea87` | Testo UI principale |
| `PopupBackground` | `#ff1243` | Popup game over |
| `PopupShadow` | `#000000` | Shadow popup |

---

## Lifecycle degli stati

```
/ (menu)  →  /game (gameplay)  →  /gameover
   ↑                                    |
   └────────────────────────────────────┘ (Play Again)
```

La navigazione usa `router.navigate(path)`. Il cleanup avviene nel `leave:` hook della route `/game`.

---

## Asset audio richiesti

| File | Tipo | Volume | Loop |
|---|---|---|---|
| `music-menu.mp3` | musica | 0.7 | sì |
| `music-play.mp3` | musica | 0.9 | sì |
| `effect-ok.mp3` | SFX | default | no |
| `effect-wrong.mp3` | SFX | default | no |

---

## Fasi della migrazione

Eseguire **una fase per sessione**. Non iniziare la fase successiva senza approvazione esplicita.

| Fase | Contenuto | Stato |
|---|---|---|
| **1 — Scaffolding** | `package.json`, `tsconfig.json`, `vite.config.ts`, `index.html`, `main.ts`, `router.ts`, `AudioUnlocker.ts`, struttura directory vuota | ✅ |
| **2 — Asset pipeline** | `RellowAssets.ts`, `AssetsLoader.ts`, copia file audio/font/sprites in `public/assets/` | ✅ |
| **3 — Menu page** | `menu.ts`, `SoundManager.ts`, bottone audio unlock, navigazione verso `/game` | ⬜ |
| **4 — Scheletro di gioco** | `gamebootstrap.ts` (app init, resize, ticker), `Game.ts` vuoto, `ScoreRepository.ts` | ⬜ |
| **5 — Colori e griglia** | `colors.ts`, `ColorButton.ts`, `ColorButtonGrid.ts`, `WordDisplay.ts` | ⬜ |
| **6 — Game loop** | `Game.ts` completo: stato waiting/won/lost, timer, shuffle UI colors, pausa 150ms, punteggio | ⬜ |
| **7 — ProgressBar e HUD** | `ProgressBar.ts`, `ScoreText.ts` con popup | ⬜ |
| **8 — Game over page** | `gameover.ts`, record check, bottone Play Again | ⬜ |
| **9 — Audio in-game** | Integrazione `SoundManager` nel game loop (win/lose/music) | ⬜ |
| **10 — Polish e test** | Responsive su mobile reale, audio unlock Safari, animazioni floating text | ⬜ |

---

## Checklist per ogni sessione

Prima di iniziare a scrivere codice:
- [ ] Ho letto `migration-specs/ARCHITECTURE_MAP.md`
- [ ] Ho controllato `migration-specs/PATTERN_COOKBOOK.md` per i pattern applicabili
- [ ] Ho controllato il file corrispondente nel progetto riferimento InfartGame Web

Prima di considerare un componente completato:
- [ ] Nessun `any` implicito
- [ ] Responsive: usa coordinate virtuali 1080×1920
- [ ] Cleanup: il componente viene rimosso correttamente quando la route cambia
- [ ] Audio: non viene chiamato prima di user gesture (o gestito con AudioUnlocker)

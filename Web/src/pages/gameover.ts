import router from './router';
import ScoreRepository from '../services/ScoreRepository';
import { SoundManagerInstance } from '../services/SoundInstance';
import { isAudioUnlocked } from '../services/AudioUnlocker';

export function renderGameOverPage(container: HTMLElement): void {
    const score  = ScoreRepository.getScore('gameover');
    const record = ScoreRepository.getScore('record');
    const isNew  = score > 0 && score === record;

    container.innerHTML = `
        <main id="gameover-root">
            <section class="gameover">
                <div class="gameover-content">
                    <h1 class="title">GAME<br>OVER</h1>

                    <div class="new-record-badge" style="${isNew ? '' : 'display:none'}">
                        &#9733; NUOVO RECORD &#9733;
                    </div>

                    <div class="gameover-scores">
                        <div class="gameover-score-item">
                            <span class="gameover-label">PUNTEGGIO</span>
                            <span class="gameover-value">${score}</span>
                        </div>
                        <div class="gameover-score-item gameover-score-item--record">
                            <span class="gameover-label">RECORD</span>
                            <span class="gameover-value">${record}</span>
                        </div>
                    </div>

                    <nav class="menu-actions">
                        <button id="btn-play-again" class="button">GIOCA ANCORA</button>
                        <button id="btn-menu" class="button button--secondary">MENU</button>
                    </nav>
                </div>
            </section>
        </main>
    `;

    container.querySelector('#btn-play-again')!.addEventListener('click', () => {
        router.navigate('/game');
    });

    container.querySelector('#btn-menu')!.addEventListener('click', () => {
        router.navigate('/');
    });

    if (isAudioUnlocked()) SoundManagerInstance.playMenuMusic();
}

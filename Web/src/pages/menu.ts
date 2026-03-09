import router from './router';
import { SoundManagerInstance } from '../services/SoundInstance';
import { unlockHowler, isAudioUnlocked } from '../services/AudioUnlocker';

export function renderMenuPage(container: HTMLElement): void {
    // Rimuovi eventuale istanza precedente
    document.getElementById('menu-root')?.remove();

    container.innerHTML = `
<main id="menu-root">
    <section class="menu">
        <div class="menu-content">
            <h1 class="title">RELLOW</h1>
            <p class="menu-subtitle">Sei più veloce del tuo cervello?</p>
            <nav class="menu-actions">
                <button id="btn-play" class="button">GIOCA</button>
                <button id="btn-audio" class="button button--secondary">ATTIVA AUDIO</button>
            </nav>
        </div>
    </section>
</main>
`;

    const audioBtn = container.querySelector<HTMLButtonElement>('#btn-audio');
    if (audioBtn) {
        if (isAudioUnlocked()) {
            audioBtn.textContent = 'Audio attivo ✓';
            audioBtn.classList.add('disabled');
        } else {
            audioBtn.addEventListener('click', async () => {
                await unlockHowler();
                audioBtn.textContent = 'Audio attivo ✓';
                audioBtn.classList.add('disabled');
                SoundManagerInstance.playMenuMusic();
            });
        }
    }

    const playBtn = container.querySelector<HTMLButtonElement>('#btn-play');
    if (playBtn) {
        playBtn.addEventListener('click', () => {
            SoundManagerInstance.stopAll();
            router.navigate('/game');
        });
    }

    if (isAudioUnlocked()) {
        SoundManagerInstance.playMenuMusic();
    }
}

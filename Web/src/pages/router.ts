import Navigo from 'navigo';
import { renderMenuPage } from './menu';
import { initGame } from './gamebootstrap';
import { renderGameOverPage } from './gameover';
import { SoundManagerInstance } from '../services/SoundInstance';

const router = new Navigo('/', {
    hash: true, // Fondamentale per il funzionamento su Jekyll
});

export function initializeRouter() {
    const appElement = document.getElementById('app');

    if (!appElement) {
        console.error('Elemento #app non trovato!');
        return router;
    }

    router
        .on(() => renderMenuPage(appElement))
        .on('/', () => renderMenuPage(appElement!), {
            leave: (done) => {
                document.getElementById('menu-root')?.remove();
                SoundManagerInstance.stopAll();
                done();
            }
        })
        .on(
            '/game',
            () => initGame(document.getElementById('app')!),
            {
                leave: (done) => {
                    import('./gamebootstrap').then(m => {
                        m.destroyGame?.();
                        done();
                    }).catch(() => done());
                }
            })
        .on('/gameover', () => renderGameOverPage(appElement!))
        .notFound(() => renderMenuPage(appElement!))
        .resolve();

    router.updatePageLinks();

    return router;
}

export default router;

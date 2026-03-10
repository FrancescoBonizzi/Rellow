using Microsoft.Xna.Framework.Audio;
using Rellow.Assets;
using System.Collections.Generic;

namespace Rellow
{
    public class SoundManager
    {
        private AssetsLoader _assetsLoader;
        private List<SoundEffectInstance> _all_sounds;

        private readonly SoundEffectInstance _winSound;
        private readonly SoundEffectInstance _looseSound;
        private readonly SoundEffectInstance _menu;
        private readonly SoundEffectInstance _playing;

        public SoundManager(AssetsLoader assetsLoader)
        {
            _assetsLoader = assetsLoader;

            _all_sounds = new List<SoundEffectInstance>();

            _winSound = assetsLoader.Sounds["effect-win"].CreateInstance();
            _all_sounds.Add(_winSound);

            _looseSound = assetsLoader.Sounds["effect-loose"].CreateInstance();
            _all_sounds.Add(_looseSound);

            _menu = assetsLoader.Sounds["music-menu"].CreateInstance();
            _menu.IsLooped = true;
            _menu.Volume = 0.7f;
            _all_sounds.Add(_menu);

            _playing = assetsLoader.Sounds["music-playing"].CreateInstance();
            _playing.IsLooped = true;
            _playing.Volume = 0.9f;
            _all_sounds.Add(_playing);
        }

        public void PlayWin()
            => _winSound.Play();

        public void PlayLoose()
            => _looseSound.Play();

        public void PlayMenu()
            => _menu.Play();

        public void PlayPlaying()
            => _playing.Play();

        public void StopSounds()
        {
            foreach (var s in _all_sounds)
            {
                s.Stop();
            }
        }

        public void PauseAll()
        {
            foreach (var s in _all_sounds)
            {
                if (s.State == SoundState.Playing)
                    s.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var s in _all_sounds)
            {
                if (s.State == SoundState.Paused)
                    s.Play();
            }
        }
    }
}

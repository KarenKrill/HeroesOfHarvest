using UnityEngine;

namespace HeroesOfHarvest.Abstractions
{
    public interface IAudioController
    {
        float MasterVolume { get; set; }
        float MusicVolume { get; set; }
        float SfxVolume { get; set; }

        void PlaySfx(AudioClip audioClip);
        void PlayMusic(AudioClip audioClip);
        void PauseMusic();
        void ResumeMusic();
        void StopMusic();
    }
}

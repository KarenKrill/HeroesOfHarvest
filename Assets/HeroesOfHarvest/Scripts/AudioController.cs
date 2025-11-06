using UnityEngine;
using UnityEngine.Audio;

using HeroesOfHarvest.Abstractions;
using Zenject;

namespace HeroesOfHarvest
{
    public class AudioController : MonoBehaviour, IAudioController
    {
        public float MasterVolume
        {
            get
            {
                if (_audioMixer.GetFloat(_masterVolumeParameter, out var decibels))
                {
                    return DecibelsToVolume(decibels);
                }
                return 0;
            }
            set
            {
                _audioMixer.SetFloat(_masterVolumeParameter, VolumeToDecibels(value));
            }
        }
        public float MusicVolume
        {
            get
            {
                if (_audioMixer.GetFloat(_musicVolumeParameter, out var decibels))
                {
                    return DecibelsToVolume(decibels);
                }
                return 0;
            }
            set
            {
                _audioMixer.SetFloat(_musicVolumeParameter, VolumeToDecibels(value));
            }
        }
        public float SfxVolume
        {
            get
            {
                if (_audioMixer.GetFloat(_sfxVolumeParameter, out var decibels))
                {
                    return DecibelsToVolume(decibels);
                }
                return 0;
            }
            set
            {
                _audioMixer.SetFloat(_sfxVolumeParameter, VolumeToDecibels(value));
            }
        }

        [Inject]
        public void Initialize(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public void PlaySfx(AudioClip audioClip) => _sfxSource.PlayOneShot(audioClip);
        public void PlayMusic(AudioClip audioClip)
        {
            _musicSource.clip = audioClip;
            _musicSource.Play();
        }
        public void PauseMusic() => _musicSource.Pause();
        public void ResumeMusic() => _musicSource.UnPause();
        public void StopMusic() => _musicSource.Stop();

        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private string _masterVolumeParameter = "MasterVolume";
        [SerializeField]
        private string _musicVolumeParameter = "MusicVolume";
        [SerializeField]
        private string _sfxVolumeParameter = "SfxVolume";
        [SerializeField]
        private AudioSource _musicSource;
        [SerializeField]
        private AudioSource _sfxSource;
        
        GameSettings _gameSettings;

        private void Start()
        {
            // dirty trick to initialize volume at start (AudioController shouldn't known about GameSettings)
            MasterVolume = _gameSettings.MusicVolume;
        }

        private float VolumeToDecibels(float volume)
        {
            if (volume <= 0.0001f)
            {
                return -80f; // Minimum volume in decibels
            }
            return 20f * Mathf.Log10(volume);
        }
        private float DecibelsToVolume(float decibels)
        {
            return Mathf.Pow(10f, decibels / 20f);
        }
    }
}

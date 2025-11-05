using System;
using UnityEngine;

namespace HeroesOfHarvest.Abstractions
{
    public enum QualityLevel
    {
        Low,
        Middle,
        High
    }

    public class GameSettings
    {
        #region Graphics

        public QualityLevel QualityLevel
        {
            get => _qualityLevel;
            set
            {
                if (_qualityLevel != value)
                {
                    _qualityLevel = value;
                    QualityLevelChanged?.Invoke(_qualityLevel);
                }
            }
        }

        #endregion

        #region Music

        [Range(0, 1)]
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (_musicVolume != value)
                {
                    _musicVolume = value;
                    MusicVolumeChanged?.Invoke(_musicVolume);
                }
            }
        }

        #endregion

        #region Diagnostic

        public bool ShowFps
        {
            get => _showFps;
            set
            {
                if (_showFps != value)
                {
                    _showFps = value;
                    ShowFpsChanged?.Invoke(_showFps);
                }
            }
        }

        #endregion

#nullable enable

        public event Action<QualityLevel>? QualityLevelChanged;

        public event Action<float>? MusicVolumeChanged;

        public event Action<bool>? ShowFpsChanged;

#nullable restore

        public GameSettings(QualityLevel qualityLevel = QualityLevel.High, float musicVolume = 1, bool showFps = true)
        {
            _qualityLevel = qualityLevel;
            _musicVolume = musicVolume;
            _showFps = showFps;
        }

        private QualityLevel _qualityLevel;
        private float _musicVolume;
        private bool _showFps;
    }
}

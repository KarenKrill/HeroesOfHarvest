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

    [Serializable]
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
                    if (!_isSettingsChangedFreezed)
                    {
                        QualityLevelChanged?.Invoke(_qualityLevel);
                    }
                    OnSettingsChanged();
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
                    if (!_isSettingsChangedFreezed)
                    {
                        MusicVolumeChanged?.Invoke(_musicVolume);
                    }
                    OnSettingsChanged();
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
                    if (!_isSettingsChangedFreezed)
                    {
                        ShowFpsChanged?.Invoke(_showFps);
                    }
                    OnSettingsChanged();
                }
            }
        }

        #endregion

#nullable enable

        public event Action<QualityLevel>? QualityLevelChanged;

        public event Action<float>? MusicVolumeChanged;

        public event Action<bool>? ShowFpsChanged;

        public event Action? SettingsChanged;

#nullable restore

        public GameSettings(QualityLevel qualityLevel = QualityLevel.High, float musicVolume = 0, bool showFps = false)
        {
            _qualityLevel = qualityLevel;
            _musicVolume = musicVolume;
            _showFps = showFps;
        }

        public bool FreezeSettingsChanged
        {
            set
            {
                if (_isSettingsChangedFreezed != value)
                {
                    _isSettingsChangedFreezed = value;
                    if (!value && _isDirty)
                    {
                        _isDirty = false;
                        SettingsChanged?.Invoke();
                    }
                }
            }
        }

        [SerializeField]
        private QualityLevel _qualityLevel;
        [SerializeField]
        private float _musicVolume;
        [SerializeField]
        private bool _showFps;
        private bool _isSettingsChangedFreezed = false;
        private bool _isDirty = false;

        private void OnSettingsChanged()
        {
            if (!_isSettingsChangedFreezed)
            {
                SettingsChanged?.Invoke();
            }
            else
            {
                _isDirty = true;
            }
        }
    }
}

using System;

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

        public event Action<bool>? ShowFpsChanged;

#nullable restore

        public GameSettings(QualityLevel qualityLevel = QualityLevel.High, bool showFps = true)
        {
            _qualityLevel = qualityLevel;
            _showFps = showFps;
        }

        private QualityLevel _qualityLevel;
        private bool _showFps;
    }
}

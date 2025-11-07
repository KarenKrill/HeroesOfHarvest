using Cysharp.Threading.Tasks;
using HeroesOfHarvest.Abstractions;
using System.Threading;
using UnityEngine;

namespace HeroesOfHarvest
{
    public class SaveService : ISaveService
    {
        public SaveService(GameSettings gameSettings,IPlayerSession playerSession, IMapObjectRegistry mapObjectRegistry)
        {
            _gameSettings = gameSettings;
            _playerSession = playerSession;
            _serializableMapObjectRegistry = mapObjectRegistry as IStringSerializable;
            SubscribeOnChanges();
        }

        public void RunInBackground()
        {
#if UNITY_WEBGL
            _cts?.Dispose();
            _cts = new();
            _ = UniTask.RunOnThreadPool(() =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    UniTask.WaitForSeconds(1);
                    if (_isSettingsOrResourcesChanged || UpdateIfMapRegistryChanged())
                    {
                        _isSettingsOrResourcesChanged = false;
                        UniTask.SwitchToMainThread();
                        PlayerPrefs.Save();
                        UniTask.SwitchToThreadPool();
                    }
                }
            }).ContinueWith(() =>
            {
                _cts.Dispose();
                _cts = null;
            });
#endif
        }

        public void Stop()
        {
#if UNITY_WEBGL
            _cts?.Cancel();
#else
            UpdateIfMapRegistryChanged();
#endif
        }

        private readonly GameSettings _gameSettings;
        private readonly IPlayerSession _playerSession;
        private readonly IStringSerializable _serializableMapObjectRegistry;
        private bool _isSettingsOrResourcesChanged;
        private string _prevSerializedMapObjectRegistry;
        private CancellationTokenSource _cts;

        private void SubscribeOnChanges()
        {
            _gameSettings.QualityLevelChanged += (newQualityLevel) =>
            {
                PlayerPrefs.SetInt("Settings.Graphics.QualityLevel", (int)newQualityLevel);
#if UNITY_WEBGL
                _isSettingsOrResourcesChanged = true;
#endif
            };
            _gameSettings.ShowFpsChanged += (newShowFps) =>
            {
                PlayerPrefs.SetInt("Settings.Diagnostics.ShowFps", newShowFps ? 1 : 0);
#if UNITY_WEBGL
                _isSettingsOrResourcesChanged = true;
#endif
            };
            _gameSettings.MusicVolumeChanged += (newMusicVolume) =>
            {
                PlayerPrefs.SetFloat("Settings.Music.MusicVolume", newMusicVolume);
#if UNITY_WEBGL
                _isSettingsOrResourcesChanged = true;
#endif
            };
            _playerSession.ResourceManager.ResourceChanged += (type, amount) =>
            {
                PlayerPrefs.SetInt($"PlayerSession.{nameof(IPlayerSession.ResourceManager)}.{type}", amount);
#if UNITY_WEBGL
                _isSettingsOrResourcesChanged = true;
#endif
            };
        }
        /// <summary>
        /// Too heavy function, but I don't have time to refactor it
        /// </summary>
        /// <returns></returns>
        private bool UpdateIfMapRegistryChanged()
        {
            var serializedMapObjectRegistry = _serializableMapObjectRegistry.ToSerializedString();
#if UNITY_WEBGL
            if (_prevSerializedMapObjectRegistry != serializedMapObjectRegistry)
            {
                _prevSerializedMapObjectRegistry = serializedMapObjectRegistry;
#endif
                PlayerPrefs.SetString("MapObjectRegistry", serializedMapObjectRegistry);
                return true;
#if UNITY_WEBGL
            }
            return false;
#endif
        }
    }
}

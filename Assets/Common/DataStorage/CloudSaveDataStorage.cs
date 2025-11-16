using System;
using System.Collections.Generic;
using System.Linq;
#if !UNITY_WEBGL
using System.Threading;
#endif
using System.Threading.Tasks;

using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

using Cysharp.Threading.Tasks;

namespace KarenKrill.DataStorage
{
    using Abstractions;

    public class CloudSaveDataStorage : IDataStorage
    {
        public CloudSaveDataStorage(ILogger logger)
        {
            _logger = logger;
        }
        public async Task SignUpAsync(string login, string password)
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(login, password);
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
        public async Task SignInAsync(string login, string password)
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(login, password);
                }
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
        public async Task SignInAnonymouslyAsync()
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    AuthenticationService.Instance.Expired += () => Debug.LogWarning("Auth expired");
                    AuthenticationService.Instance.SignedIn += () => Debug.Log("Auth signed in");
                    AuthenticationService.Instance.SignedOut += () => Debug.Log("Auth signed out");
                    AuthenticationService.Instance.SignInFailed += ex => Debug.Log($"Auth SignIn failed: {ex}");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
#if UNITY_WEBGL
        public Task SignOutAsync()
        {
#else
        public async Task SignOutAsync()
        {
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    AuthenticationService.Instance.SignOut();
                }
#if UNITY_WEBGL
                return Task.CompletedTask;
#else
        }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }

        #region IDataStorage implementation

        public async Task InitializeAsync()
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
                var startThreadId = Thread.CurrentThread.ManagedThreadId;
                await UniTask.SwitchToMainThread();
                var mainThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
                await InitServicesIfNeeded();
                await SignInServicesIfNeeded();
#if !UNITY_WEBGL
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
#endif
                AuthenticationService.Instance.Expired -= OnSessionExpired;
                AuthenticationService.Instance.Expired += OnSessionExpired;
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
        public async Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata) =>
            await LoadUniAsync(metadata).AsTask();

        public async UniTask<IDictionary<string, object>> LoadUniAsync(IDictionary<string, Type> metadata)
        {
#if !UNITY_WEBGL
            var startThreadId = Thread.CurrentThread.ManagedThreadId;
            await UniTask.SwitchToMainThread();
            var mainThreadId = Thread.CurrentThread.ManagedThreadId;
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                var dataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>(metadata.Keys));
#if !UNITY_WEBGL
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
#endif
                Dictionary<string, object> result = new();
                var getAsTypeMethodInfo = typeof(Unity.Services.CloudSave.Internal.Http.IDeserializable).GetMethod(nameof(Unity.Services.CloudSave.Internal.Http.IDeserializable.GetAs));
                foreach (var itemMetadata in metadata)
                {
                    if (dataItems.TryGetValue(itemMetadata.Key, out var item))
                    {
                        var getAsGenericTypeMethodInfo = getAsTypeMethodInfo.MakeGenericMethod(itemMetadata.Value);
                        var obj = getAsGenericTypeMethodInfo?.Invoke(item.Value, _deserializationParams);
                        result[item.Key] = obj;
                    }
            }
#if !UNITY_WEBGL
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToMainThread();
                }
#endif
                return result;
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
        public async Task SaveAsync(IDictionary<string, object> data)
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
                var startThreadId = Thread.CurrentThread.ManagedThreadId;
                await UniTask.SwitchToMainThread();
                var mainThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
#if !UNITY_WEBGL
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }

#endregion

        private readonly ILogger _logger;
#if !UNITY_WEBGL
        private readonly SemaphoreSlim _signinSemaphore = new(1, 1);
#endif
        private readonly object[] _deserializationParams = new object[1] { null };

        private async Task InitServicesIfNeeded()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
        }
        private async Task SignInServicesIfNeeded()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        private void OnSessionExpired() => OnSessionExpiredAsync().AsUniTask().Forget();
        private async Task OnSessionExpiredAsync()
        {
#if !UNITY_WEBGL
            await _signinSemaphore.WaitAsync();
            try
            {
#endif
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
#if !UNITY_WEBGL
            }
            finally
            {
                _signinSemaphore.Release();
            }
#endif
        }
    }
}

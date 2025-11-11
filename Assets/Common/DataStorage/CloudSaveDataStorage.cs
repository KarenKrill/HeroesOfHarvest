using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task SignUpAsync(string login, string password)
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(login, password);
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }
        public async Task SignInAsync(string login, string password)
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(login, password);
                }
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }
        public async Task SignInAnonymouslyAsync()
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    AuthenticationService.Instance.Expired += () => Debug.LogWarning("Auth expired");
                    AuthenticationService.Instance.SignedIn += () => Debug.Log("Auth signed in");
                    AuthenticationService.Instance.SignedOut += () => Debug.Log("Auth signed out");
                    AuthenticationService.Instance.SignInFailed += ex => Debug.Log($"Auth SignIn failed: {ex}");
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }
        public async Task SignOut()
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    AuthenticationService.Instance.SignOut();
                }
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }

        #region IDataStorage implementation

        public async Task InitializeAsync()
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                var startThreadId = Thread.CurrentThread.ManagedThreadId;
                await UniTask.SwitchToMainThread();
                var mainThreadId = Thread.CurrentThread.ManagedThreadId;
                await InitServicesIfNeeded();
                await SignInServicesIfNeeded();
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
                AuthenticationService.Instance.Expired -= OnSessionExpired;
                AuthenticationService.Instance.Expired += OnSessionExpired;
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }
        public async Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata)
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                var startThreadId = Thread.CurrentThread.ManagedThreadId;
                await UniTask.SwitchToMainThread();
                var mainThreadId = Thread.CurrentThread.ManagedThreadId;
                var dataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>(metadata.Keys.AsEnumerable()));
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
                Dictionary<string, object> result = new();
                var getAsTypeMethodInfo = typeof(Unity.Services.CloudSave.Internal.Http.IDeserializable).GetMethod(nameof(Unity.Services.CloudSave.Internal.Http.IDeserializable.GetAs));
                foreach (var itemMetadata in metadata)
                {
                    if (dataItems.TryGetValue(itemMetadata.Key, out var item))
                    {
                        var getAsGenericTypeMethodInfo = getAsTypeMethodInfo.MakeGenericMethod(itemMetadata.Value);
                        var obj = getAsGenericTypeMethodInfo?.Invoke(item.Value, new object[1] { null });
                        result[item.Key] = obj;
                    }
                }
                return result;
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }
        public async Task SaveAsync(IDictionary<string, object> data)
        {
            await _signinSemaphore.WaitAsync();
            try
            {
                var startThreadId = Thread.CurrentThread.ManagedThreadId;
                await UniTask.SwitchToMainThread();
                var mainThreadId = Thread.CurrentThread.ManagedThreadId;
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                if (startThreadId != mainThreadId)
                {
                    await UniTask.SwitchToThreadPool();
                }
            }
            finally
            {
                _signinSemaphore.Release();
            }
        }

        #endregion

        private readonly SemaphoreSlim _signinSemaphore = new(1, 1);

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
        private void OnSessionExpired()
        {
            _ = Task.Run(async () =>
            {
                await _signinSemaphore.WaitAsync();
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                finally
                {
                    _signinSemaphore.Release();
                }
            });
        }
    }
}

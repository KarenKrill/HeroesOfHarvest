using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using KarenKrill.DataStorage.Abstractions;

namespace KarenKrill.DataStorage
{
    public class CloudSaveDataStorage : IDataStorage
    {
        public async Task SignUpAsync(string login, string password)
        {
            await InitServicesIfNeeded();
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(login, password);
        }
        public async Task SignInAsync(string login, string password)
        {
            await InitServicesIfNeeded();
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(login, password);
        }
        public async Task SignInAnonymouslyAsync()
        {
            await InitServicesIfNeeded();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        public void SignOut() => AuthenticationService.Instance.SignOut();

        #region IDataStorage implementation

        public async Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata)
        {
            await InitServicesIfNeeded();
            var dataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>(metadata.Keys.AsEnumerable()));
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
        public async Task SaveAsync(IDictionary<string, object> data)
        {
            await InitServicesIfNeeded();
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        #endregion

        private async Task InitServicesIfNeeded()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KarenKrill.DataStorage.Abstractions
{
    public interface IDataStorage
    {
        Task InitializeAsync();
        Task<IDictionary<string, object>> LoadAsync(IDictionary<string, Type> metadata);
        Task SaveAsync(IDictionary<string, object> data);
    }
}

using UnityEngine;
using UnityEngine.Pool;

namespace KarenKrill.Instantiattion
{
    public class ComponentPool<T> : IObjectPool<T>, System.IDisposable where T : Component
    {
        public int CountInactive => _pool.CountInactive;

        public ComponentPool(T prefab, Transform parent, int capacity = 5, int maxSize = 1000, bool collectionCheck = false)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new(
                createFunc: () => Object.Instantiate(_prefab, _parent),
                actionOnGet: component => component.gameObject.SetActive(true),
                actionOnRelease: component => component.gameObject.SetActive(false),
                actionOnDestroy: component => Object.Destroy(component.gameObject),
                collectionCheck: collectionCheck,
                defaultCapacity: capacity,
                maxSize: maxSize
            );
        }
        public T Get() => _pool.Get();
        public PooledObject<T> Get(out T item) => _pool.Get(out item);
        public void Release(T item) => _pool.Release(item);
        public void Clear() => _pool.Clear();
        public void Dispose() => _pool.Dispose();

        private readonly Transform _parent;
        private readonly T _prefab;
        private readonly ObjectPool<T> _pool;
    }
}

using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolBase<T> where T : Component
{
    private T _prefab;
    private ObjectPool<T> _pool;

    private ObjectPool<T> Pool
    {
        get
        {
            return _pool;
        }
        set => _pool = value;
    }

    public ObjectPoolBase(T prefab, int initial = 50, int max = 500, bool collectionChecks = false)
    {
        InitPool(prefab);
    }

    protected void InitPool(T prefab, int initial = 50, int max = 500, bool collectionChecks = false)
    {
        _prefab = prefab;
        Pool = new ObjectPool<T>(
            InstantiatePoolableObject,
            OnGet,
            OnRelease,
            DestroySetup,
            collectionChecks,
            initial,
            max);
    }

    protected virtual T InstantiatePoolableObject() => MonoBehaviour.Instantiate(_prefab);
    protected virtual void OnGet(T obj) => obj.gameObject.SetActive(true);
    protected virtual void OnRelease(T obj) => obj.gameObject.SetActive(false);
    protected virtual void DestroySetup(T obj) => MonoBehaviour.Destroy(obj);

    public T Get() => Pool.Get();
    public void Release(T obj) => Pool.Release(obj);

}
#region Script Synopsis
    //ObjectPool is a object pooling class that stores, adds and removes objects.
    //Examples: FireBullet.Pool, GlobalShotManger.explosionPool

    //IPooler is used to determine what can act as a pooling source for storing the pool.
    //Examples: FireBullet, GlobalShotMager

    //IRePoolable is used to determine what can be pooled.
    //Examples: Most bullet type shot scripts.

    //Learn more about object pooling at: https://neondagger.com/variabullet2d-system-guide/#object-pooling
#endregion

using UnityEngine;
using System.Collections.Generic;

namespace ND_VariaBULLET
{
    public class ObjectPool
    {
        public List<GameObject> list = new List<GameObject>();
        public int Size {
            get { return list.Count; }
        }

        public ObjectPool() { }

        public ObjectPool(GameObject pooledObject, Transform transform)
        {
            AddToPool(pooledObject, transform);
        }

        public void AddToPool(GameObject pooledObject, Transform transform)
        {
            pooledObject.SetActive(false);
            pooledObject.transform.parent = transform;
            list.Add(pooledObject);
        }

        public GameObject RemoveFromPool(int index)
        {
            GameObject pooledObject = list[index];
            pooledObject.SetActive(true);
            list.RemoveAt(index);
            return pooledObject;
        }
    }

    public interface IPooler
    {
        bool PoolingEnabled { get; set; }
        void AddToPool(GameObject poolObject, Transform parent);
        GameObject RemoveFromPool(int index);
    }

    public interface IRePoolable
    {
        void RePool(IPooler poolingScript);
    }
}
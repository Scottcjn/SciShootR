#region Script Synopsis
    //A "Singleton pattern" MonoBehavior which is force-instantiated by GlobalShotManager.
    //Handles storage of banked (globally pooled) shots. These shots are not destroyed when their emitters are destroyed.
    //Optionally pre-banks user-defined common shots types ahead of time.
    //Shot banking is set at the emitter level through attached Firing script.
#endregion

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ND_VariaBULLET
{
    public class GlobalShotBank : MonoBehaviour, IPooler
    {   
        [Tooltip("Optional pre-pooled (banked) shots available at Start.")]
        public GameObject[] PreBankedShots;

        [Range(1, 200)]
        [Tooltip("Limits the size of pre-pooled (banked) shots for each shot to this number.")]
        public int PreBankSize;

        private Dictionary<string, ObjectPool> Pool = new Dictionary<string, ObjectPool>();

        [Tooltip("Sets the total limit of banked shots. [when limit is reached shots default to their emitter-level pool settings].")]
        public int PoolMaxSize = 1000;

        public int PoolCount
        {
            get
            {
                return Pool.Values.Sum(
                    (pool) => pool.list.Count
                );
            }
        }

        private static GlobalShotBank _instance;
        public static GlobalShotBank Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Instantiate(Resources.Load<GameObject>("ND_VariaBullet/GlobalManager/GlobalShotBank")).GetComponent<GlobalShotBank>();

                return _instance;
            }
        }

        public bool PoolingEnabled
        {
            get { return true; }
            set { PoolingEnabled = value; } //necessary to satisfy IPooler. Pooling can only be true in this case
        }

        void Awake()
        {
            foreach (GameObject shot in PreBankedShots)
            {
                Pool.Add(shot.name, new ObjectPool());

                for (int i = 0; i < PreBankSize; i++)
                {
                    GameObject copy = Instantiate(shot);
                    AddToPool(copy, this.transform);
                }
            }
        }

        public void ForceInstantiate()
        {
            //no implementation, simply forces instantiation when called
        }

        public void ClearBank()
        {
            //called elswhere in code to force clearing of entire bank
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            Pool = new Dictionary<string, ObjectPool>();
        }

        public void AddToPool(GameObject poolObject, Transform parent)
        {
            if (!Pool.ContainsKey(poolObject.name))
                Pool.Add(poolObject.name, new ObjectPool());

            Pool[poolObject.name].AddToPool(poolObject, parent);
        }

        public GameObject RemoveFromPool(string name, int index)
        {
            return Pool[name + "(Clone)"].RemoveFromPool(0);
        }

        public GameObject RemoveFromPool(int index)
        {
            throw new NotImplementedException("Requires key name to be present. Use RemoveFromPool(string name, int index) instead.");
        }

        public bool ContainsShot(string name)
        {
            return Pool.ContainsKey(name + "(Clone)") && Pool[name + "(Clone)"].list.Count > 0;
        }
    }
}
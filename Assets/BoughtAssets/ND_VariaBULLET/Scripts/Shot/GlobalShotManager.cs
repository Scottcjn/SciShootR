#region Script Synopsis
    //A "Singleton pattern" MonoBehavior which is lazily instantiated whenever a shot is fired.
    //Alternatively, instantiation is forced when directly called from ForceGlobalShotManager when attached to a persistent scene gameobject.
    //Main role is in managing global settings such as target FPS, explosions and global shot speed and engine throttling.
    //Learn more about the GlobalShotManager at: https://neondagger.com/variabullet2d-system-guide/#globalshotmanager
#endregion

using UnityEngine;
using System.Collections.Generic;
using System;

namespace ND_VariaBULLET
{
    public class GlobalShotManager : MonoBehaviour, IPooler
    {       
        public Transform StaticCollTransform { get; set; }
        public Camera MainCam { get; private set; }

        [Header("Static Collision Source")]

        [SerializeField]
        [Tooltip("Sets the method by which static collider object is found in the scene. [Object becomes the collision source for shots with CollFlux enabled].")]
        private StaticCollRef FindStaticCollBy;

        [SerializeField]
        [Tooltip("Sets the name of the static collider object that FindStaticCollBy looks for.")]
        private string StaticColl;

        [Range(0.5f, 99)]
        [Tooltip("Sets the radius in units within which the static collider object dynamically enables colliders of surrounding shots that have CollFlux enabled.")]
        public float StaticCollRadius = 5;

        [Tooltip("Enables using demo version GlobalShotManager. Should only be used by VariaBULLET2D Demo Scenes.")]
        public static bool DemoMode;

        private static GlobalShotManager _instance;
        public static GlobalShotManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    string folder = (DemoMode) ? "GlobalManager/DemoVersion" : "GlobalManager";
                    _instance = Instantiate(Resources.Load<GameObject>("ND_VariaBullet/" + folder + "/GlobalShotManager")).GetComponent<GlobalShotManager>();
                }
                
                return _instance;
            }
        }


        [Header("FPS Testing")]

        [Tooltip("Disables framerate testing when turned off.")]
        public bool EnableTestFrameRate;

        [Range(1, 60)]
        [Tooltip("Sets Target Framerate if EnableTestFrameRate is toggled.")]
        public int TestFrameRate = 60;

        [Tooltip("Enables VSync.")]
        public bool VSync;


        [Header("Global Shot Scaling")]

        [Range(0.1f, 5)]
        [Tooltip("Sets global speed scale for all shots. [individual shots can ingore via IgnoreGlobalSpeedScale field].")]
        public float SpeedScale = 1;

        [Range(0.1f, 5)]
        [Tooltip("Sets global rate scale for all shot emitters. [individual emitters can ingore via IgnoreGlobalRateScale field].")]
        public float RateScale = 1;

        [Tooltip("Sets whether or not RateScale automatically adjust to that or SpeedScale.")]
        public bool LockRateToSpeed;


        [Header("Explosion Pool")]

        [Tooltip("Sets explosion prefabs which are requested by collision scripts.")]
        public GameObject[] ExplosionPrefabs;

        public bool PoolingEnabled
        {
            get { return true; }
            set { PoolingEnabled = value; } //necessary to satisfy IPooler. Pooling can only be true in this case
        }
       
        [Range(1, 100)]
        [Tooltip("Limits the size of pre-pooled explosions for each explosion to this number.")]
        public int PoolSize;


        [Header("Throttle Emulation")]

        [Tooltip("Enables emulation of CPU throttling, resulting in deterministic slowdown when MaxBulletsUntilThrottle is reached.")]
        public bool EmulateCPUThrottle;

        [Tooltip("Sets the amount of throttling (slowdown) each bullets creates when EmulateCPUThrottle is used.")]
        public float ThrottlePerBullet;

        [Range(0.1f, 0.9f)]
        [Tooltip("Sets a limit on the max amount of throttling (slowdown) the engine can emulate as a percentage. [higher = slower].")]
        public float MaxThrottle;

        [Tooltip("Sets the threshold of total active bullets at which begins engine throttling (slowdown).")]
        public int MaxBulletUntilThrottle;


        [Header("Global Boundary")]

        [Tooltip("Sets the range outside of the camera view, where shots are to be destroyed or re-pooled. [larger number = farther from camera bounds]")]
        public Vector2 OutBoundsRange = new Vector2(260,260);

        public int ActiveBullets { get; set; }

        private Dictionary<string, ObjectPool> explosionPool = new Dictionary<string, ObjectPool>();
        private Dictionary<string, AudioSource> sfxPool = new Dictionary<string, AudioSource>();

        void Awake()
        {
            GlobalShotBank.Instance.ForceInstantiate();

            getStaticColl();
            initExplosionPool();

            MainCam = Camera.main;
        }

        public void Update()
        {
            if (EnableTestFrameRate)
                setFrameRate();

            if (EmulateCPUThrottle)
                setThrottle();

            if (LockRateToSpeed)
                RateScale = SpeedScale;
        }


        private void setFrameRate()
        {
            Application.targetFrameRate = TestFrameRate;
            QualitySettings.vSyncCount = (VSync) ? 1 : 0;
        }

        private void setThrottle()
        {
            if (Time.timeScale == 0)
                return;

            int difference = Math.Max(0, ActiveBullets - MaxBulletUntilThrottle);
            float throttle = (float)difference * ThrottlePerBullet;
            Time.timeScale = (throttle > MaxThrottle) ? 1 - MaxThrottle : 1 - throttle;
        }

        private void getStaticColl()
        {
            if (!String.IsNullOrEmpty(StaticColl))
            {
                GameObject gO;

                if (FindStaticCollBy == StaticCollRef.Name)
                    gO = GameObject.Find(StaticColl);
                else
                    gO = GameObject.FindGameObjectWithTag(StaticColl);

                if (gO != null)
                    StaticCollTransform = gO.transform;
            }
        }

        private void initExplosionPool()
        {
            foreach (GameObject explosion in ExplosionPrefabs)
            {
                explosionPool.Add(explosion.name, new ObjectPool());

                Explosion eScript = explosion.GetComponent<Explosion>();
                if (eScript.SoundFX != null)
                {
                    AudioSource soundFX = gameObject.AddComponent<AudioSource>();
                    soundFX.clip = eScript.SoundFX;
                    soundFX.volume = eScript.Attenuation;
                    soundFX.playOnAwake = false;
                    sfxPool.Add(explosion.name, soundFX);
                }
            }
        }

        public GameObject ExplosionRequest(string name, object sender)
        {
            ObjectPool pooledExplosion;

            try
            {
                pooledExplosion = explosionPool[name];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("ERROR: " + sender + " requested Explosion named \"" + name +
                                               "\" was not found. Make sure the Explosion prefab name or request string is correct.");
            }

            if (pooledExplosion.Size == 0)
            {
                foreach (GameObject explosion in ExplosionPrefabs)
                {
                    if (explosion.name == name)
                    {
                        for (int i = 0; i < PoolSize; i++)
                        {
                            GameObject copy = Instantiate(explosion);
                            copy.name = name;
                            AddToPool(copy, this.transform);
                        }
                        break;
                    }
                }
            }

            if (sfxPool.ContainsKey(name))
                sfxPool[name].Play();

            return explosionPool[name].RemoveFromPool(0);
        }

        public void AddToPool(GameObject poolObject, Transform parent)
        {
            explosionPool[poolObject.name].AddToPool(poolObject, parent);
        }

        public GameObject RemoveFromPool(int index)
        {
            throw new NotImplementedException();
        }

        public enum StaticCollRef
        {
            Name,
            Tag
        }
    }
}
#region Script Synopsis
    //Subclass of FireBase for instantiating, and maintaining a pool of, bullets and is attached to an emitter point gameobject.
    //Instantiation can occur at different intervals depending on methods chosen, rate, pause and pause length counters.
    //Learn more about firing scripts at: https://neondagger.com/variabullet2d-quick-start-guide/#firing-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class FireBullet : FireBase, IPooler
    {
        [Tooltip("Overrides the fired shot's sprite.")]
        public Sprite SpriteOverride;

        [Tooltip("Sets conditions for parenting fired shot's transform to the emitter's.")]
        public ParentType ParentToEmitter;

        [Header("Rate Pattern")]

        [Tooltip("Ignores any rate scaling that has been set in GlobalShotManager.")]
        public bool IgnoreGlobalRateScale;

        [Range(200, 1)]
        [Tooltip("Sets rate of shots. [Lower number = more shots].")]
        public int ShotRate;
        private Timer shotRateCounter = new Timer(0);

        [Range(1, 100)]
        [Tooltip("Sets rate of intermittent gaps (pauses) between shots. [Lower number = more frequent gaps. 100 = no gaps].")]
        public float PauseRate;
        private Timer pauseRateCounter = new Timer(0);

        [Range(1, 100)]
        [Tooltip("Sets length of gaps produced by PauseRate.")]
        public int PauseLength;
        private Timer pauseLengthCounter = new Timer(0);

        [Tooltip("Sets whether subsequent shots from this emitter appear over or under previous ones.")]
        public ShotOverlap ShotOverlap = ShotOverlap.Over;


        [Header("Shot Pooling")]

        [SerializeField]
        [Tooltip("Enables pooling of fired shot at emitter level. Pool is destroyed when emitter is destroyed.")]
        private bool _poolingEnabled = false;
        public bool PoolingEnabled { get { return _poolingEnabled; } set { _poolingEnabled = value; } }

        [Tooltip("Automatically starts emitter with calculated set of pre-pooled shots.")]
        public bool AutoPool;

        [Tooltip("When using AutoPool, overrides pre-pooled shot amount with user defined amount.")]
        public int AutoPoolOverride;

        [Tooltip("Overrides this shot's emitter-level pooling and enables pooling at the global level [GlobalShotBank]. Pool remains when emitter is destroyed.")]
        public bool BankingEnabled;

        public ObjectPool Pool = new ObjectPool();

        private bool triggered = false;
        private int increment;

        public override void Start()
        {
            if (Utilities.IsEditorMode())
            {
                if (!makeNodeOnly)
                    checkShotMismatch(typeof(ShotLaser), "Bullet");

                return;
            }

            base.Start();
            initPool();

            if (SpriteOverride != null)
                BankingEnabled = false;
        }

        private void initPool()
        {
            if (!PoolingEnabled)
                return;

            int poolSize = 0;

            if (AutoPoolOverride > 0)
                poolSize = AutoPoolOverride;
            else if (AutoPool)
                poolSize = calcObjectPool();

            for (int i = 0; i < poolSize; i++)
            {
                var pooledObject = Instantiate(Shot) as GameObject;
                AddToPool(pooledObject, this.transform);
            }
        }

        private int calcObjectPool()
        {
            float maxBulletSpeed = speedLimit;

            float calc1 = (maxBulletSpeed - ShotSpeed) / ShotRate * 3;
            float calc2 = (float)PauseLength / maxBulletSpeed / 2;
            float calc3 = calc1 - calc1 * calc2;
            return (int)(calc3 * PauseRate / maxBulletSpeed);
        }

        protected override bool ButtonPress()
        {
            if (Input.GetKeyDown(controller.CommandKey)) { firstShotCounterReset(); return false; }

            return Input.GetKey(controller.CommandKey);
        }

        protected override bool ButtonPressAutoHold()
        {
            return base.ButtonPressAutoHold();
        }

        protected override bool AutoFire()
        {
            if (controller.TriggerAutoFire && triggered)
                return true;
            else if (controller.TriggerAutoFire && !triggered)
            {
                triggered = true;
                firstShotCounterReset();
                return true;
            }          
            else
            {
                triggered = false;
                firstShotCounterReset();
                return false;
            }
        }

        protected override bool AutoFireAutoHold()
        {
            return base.AutoFireAutoHold();
        }

        protected override bool AutoHoldTemplate(bool commandType)
        { 
            if (commandType && !AutoHold)
            {
                firstShotCounterReset();
                AutoHold = true;
            }

            if (AutoHold && !AutoHoldCounter.Flag)
            {
                AutoHoldCounter.Run(controller.AutoHoldDuration);
                return true;
            }
            else
            {
                AutoHold = false;
                controller.TriggerAutoFire = false;
                AutoHoldCounter.Reset();
                return false;
            }
        }

        protected override bool ShootAtCurrentInterval()
        {
            shotRateCounter.Run(ShotRate / (IgnoreGlobalRateScale ? 1 : GlobalShotManager.Instance.RateScale));
            pauseRateCounter.Run(PauseRate + ShotRate);

            if (PauseRate == 100)
                PauseRate = Mathf.Infinity;

            if (!pauseLengthCounter.Flag)
            {
                if (!pauseRateCounter.Flag)
                {
                    if (shotRateCounter.Flag)
                        return true;
                }
                else
                {
                    pauseLengthCounter.Run(PauseLength);
                    pauseRateCounter.ForceFlag(PauseRate + ShotRate + 1);
                }
            }
            else
            {
                shotRateCounter.Reset();
                pauseRateCounter.Reset();
                pauseLengthCounter.Reset();
            }

            return false;
        }

        private void firstShotCounterReset() //resets on initial key command, allows for immediate spawn on first trigger/buttonpress
        {       
            shotRateCounter.ForceFlag(ShotRate + 1);
            pauseRateCounter.Reset();
            pauseLengthCounter.Reset();
        }

        public override void InstantiateShot()
        {
            Player playerComponent = GetComponentInParent<Player>();
            Enemy enemyComponent = GetComponentInParent<Enemy>();

            if (playerComponent != null && playerComponent.GetCanShoot() == false)
            {
                return;
            }
            else if (enemyComponent != null && enemyComponent.GetCanShoot() == false)
            {
                return;
            }

            GameObject firedShot;

            if (Pool.list.Count > 0)
                firedShot = RemoveFromPool(0);
            else if (GlobalShotBank.Instance.ContainsShot(Shot.name))
                firedShot = GlobalShotBank.Instance.RemoveFromPool(Shot.name, 0);
            else
                firedShot = Instantiate(Shot) as GameObject;

            //Below two lines added to fix pooled bullets retaining parent localscale, resulting in incorrect 180 flipping when re-instantiated
            firedShot.transform.parent = null;
            firedShot.transform.localScale = Shot.transform.localScale;

            ShotBase shotScript = firedShot.GetComponent<ShotBase>();
            shotScript.Emitter = this.transform;
            shotScript.ShotSpeed = this.ShotSpeed;
            shotScript.Trajectory = this.angleToPercentage();
            shotScript.ExitPoint = controller.ExitPointOffset + LocalOffset;
            shotScript.FiringScript = this;
            shotScript.PoolBank = BankingEnabled;

            if (ParentToEmitter == ParentType.never)
                shotScript.ParentToEmitter = ParentType.never;
            else if (ParentToEmitter == ParentType.always)
                shotScript.ParentToEmitter = ParentType.always;
            else
                shotScript.ParentToEmitter = ParentType.whileShotHeld;

            int physicsLayer = LayerMask.NameToLayer(rend.sortingLayerName);
            firedShot.layer = physicsLayer;

            shotScript.sortLayer = rend.sortingLayerName;
            increment += (int)ShotOverlap;
            shotScript.sortOrder = increment + rend.sortingOrder - 9999;
            shotScript.InitialSet();

            if (audiosrc != null)
                audiosrc.Play();

            GlobalShotManager.Instance.ActiveBullets++;
        }

        private Vector2 angleToPercentage()
        {
            int globalDirection = (transform.lossyScale.x < 0) ? -1 : 1;
            float angle = Mathf.Abs(transform.rotation.eulerAngles.z); //absolute value fixes negative value at -360

            return CalcObject.RotationToShotVector(angle) * globalDirection;
        }

        public void AddToPool(GameObject pooledObject, Transform parent)
        {
            if (PoolingEnabled) { Pool.AddToPool(pooledObject, parent); }
        }

        public GameObject RemoveFromPool(int index)
        {
            return Pool.RemoveFromPool(index);
        }
    }

    public enum ParentType
    {
        never,
        always,
        whileShotHeld
    }

    public enum ShotOverlap
    {
        Over = 1,
        Under = -1,
    }
}
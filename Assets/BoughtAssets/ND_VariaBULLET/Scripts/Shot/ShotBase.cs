#region Script Synopsis
    //Base class for all shot scripts attached to bullet/laser prefabs.
    //Manages many common shot properties & actions.
    //Description of events and methods can be found at https://neondagger.com/variabullet2d-scripting-guide/#shot-methods-events
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    public class ShotBase : MonoBehaviour, IDamager
    {
        public bool PoolBank { get; set; }

        [HideInInspector]
        public float ShotSpeed;

        [Tooltip("Ignores any speed scaling that has been set in GlobalShotManager.")]
        public bool IgnoreGlobalSpeedScale;
        protected float scale;

        [HideInInspector]
        public ParentType ParentToEmitter;

        [Tooltip("Sets damage amount produced when this shot collides with an object that has a ShotCollisionDamage script attached.")]
        public float DamagePerHit = 1;
        public float DMG { get { return DamagePerHit; } }

        [Tooltip("Sets this shot rotation intitially to that of its emitter.")]
        public bool InheritStartRotation = true;

        [HideInInspector]
        public float scaledSpeed;

        [HideInInspector]
        public Vector2 Trajectory;

        [HideInInspector]
        public Transform Emitter;

        [HideInInspector]
        public float ExitPoint;

        [HideInInspector]
        public FireBase FiringScript;

        [HideInInspector]
        public string sortLayer;

        [HideInInspector]
        public int sortOrder;

        private bool emitterDestroyedFlag;
        private Timer eventCounter = new Timer(0);

        protected SpriteRenderer rend;
        private bool poolOrDestroyTriggered;

        public virtual void InitialSet()
        {
            eventCounter.Reset();
            poolOrDestroyTriggered = false;

            transform.parent = Emitter;
            transform.localPosition = new Vector2(ExitPoint, 0);
                     
            if (InheritStartRotation) //fix for case where parent sprite is flipped
            {
                if (Emitter.transform.lossyScale.x > 0)
                    transform.rotation = Emitter.rotation;
                else
                    transform.rotation = Quaternion.AngleAxis(Mathf.Abs(Emitter.rotation.eulerAngles.z) - 180, Vector3.forward);
            }

            if (ParentToEmitter == ParentType.whileShotHeld)
                FiringScript.OnStoppedFiring.AddListener(UnParent);
            else if (ParentToEmitter == ParentType.never)
                transform.parent = null;

            rend = GetComponent<SpriteRenderer>();
            setSprite(rend);
        }

        public virtual void Start()
        {
            //NOT IMPLEMENTED
            //Use InitialSet as default Start/Constructor unless external dependency requires Start()  
        }

        public virtual void Update()
        {
            OnOutBounds();
            scale = (IgnoreGlobalSpeedScale) ? 1 : GlobalShotManager.Instance.SpeedScale;
            scaledSpeed = ShotSpeed * scale;
        }

        protected virtual void setSprite(SpriteRenderer sr)
        {
            FireBullet fb = FiringScript as FireBullet;

            if (fb.SpriteOverride != null)
                sr.sprite = fb.SpriteOverride;

            sr.color = FiringScript.SpriteColor;
            sr.sortingLayerName = sortLayer;
            sr.sortingOrder = sortOrder;
        }

        public void OnEmitterDestroyedDo(Action<ShotBase> action)
        {
            if (Emitter == null)
                action(this);
        }

        public void OnEmitterDestroyedDoOnce(Action<ShotBase> action)
        {
            if (Emitter == null && emitterDestroyedFlag == false)
            {
                action(this);
                emitterDestroyedFlag = true;
            }
        }

        public void OnEventTimerDo(Action<ShotBase> action, int timeLimit)
        {
            if (!eventCounter.Flag)
            {
                eventCounter.Run(timeLimit);
                return;
            }

            action(this);
        }
        
        public void OnEventTimerDoOnce(Action<ShotBase> action, int timeLimit)
        {
            if (eventCounter.Flag)
                return;

            eventCounter.Run(timeLimit);

            if (eventCounter.Flag)
                action(this);
        }

        public void OnEventTimerDoRepeat(Action<ShotBase> action, int timeLimit)
        {
            if (!eventCounter.Flag)
            { eventCounter.Run(timeLimit); return; }
            else
                eventCounter.Reset();

            action(this);
        }

        protected virtual void UnParent()
        {
            FiringScript.OnStoppedFiring.RemoveListener(UnParent);
            transform.parent = null;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            RePoolOrDestroy();
        }

        protected virtual void OnOutBounds()
        {
            if (CalcObject.IsOutBounds(transform))
                RePoolOrDestroy();
        }

        protected virtual void RePoolOrDestroy()
        {
            if (poolOrDestroyTriggered)
                return;

            IPooler poolingScript;

            if (PoolBank && this is IRePoolable && !(GlobalShotBank.Instance.PoolCount > GlobalShotBank.Instance.PoolMaxSize))
            {
                poolingScript = GlobalShotBank.Instance;
                RePool(poolingScript);

                return;
            }

            if (FiringScript != null && FiringScript is IPooler)
            {
                poolingScript = FiringScript as IPooler;

                if (poolingScript.PoolingEnabled)
                {
                    if (this is IRePoolable)
                        RePool(poolingScript);
                    else
                        Kill(gameObject);
                }
                else
                    Kill(gameObject);
            }
            else
                Kill(gameObject);
        }
     
        public virtual void RePool(IPooler poolingScript) //default re-pool Behavior. Override to to accomodate custom behaviors on repool.
        {
            if (PoolBank && this is IRePoolable && !(GlobalShotBank.Instance.PoolCount > GlobalShotBank.Instance.PoolMaxSize)) //and pool available
                poolingScript.AddToPool(this.gameObject, GlobalShotBank.Instance.transform);
            else
                poolingScript.AddToPool(this.gameObject, Emitter);

            GlobalShotManager.Instance.ActiveBullets--;
            poolOrDestroyTriggered = true;
        }

        protected void Kill(GameObject gO)
        {
            Destroy(gO);
            GlobalShotManager.Instance.ActiveBullets--;
            poolOrDestroyTriggered = true;
        }
    }
}
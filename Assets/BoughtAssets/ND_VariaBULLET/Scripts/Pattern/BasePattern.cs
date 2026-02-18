#region Script Synopsis
//Base class for a SpreadPattern Controller script, mainly involved in creating emitters and triggering fire.
//Attached to the Controller gameobject to form the core behavior for the controller/emitters.
//Learn more about controllers/emitters at: http://neondagger.com/variabullet2d-quick-start-guide/#setting-emitters-controls
#endregion

using UnityEngine;
using System;
using System.Collections.Generic;

namespace ND_VariaBULLET
{
    public abstract class BasePattern : MonoBehaviour
    {
        [Tooltip("Sets this controller as master shot trigger, slaving all child controller triggers to it.")]
        public bool Master;

        [Tooltip("Disables realtime changes to emitter pattern characteristics.")]
        public bool FreezeEdits = false;

        [SerializeField]
        [Tooltip("Sets anchor transform position relative to sprite in parent Origin. Manual requires setting in Origin parent transform directly.")]
        private AutoSetOrigin origin;
        private AutoSetOrigin prevOrigin;


        [Header("Trigger Settings")]

        [Tooltip("Sets the command type that triggers shots for this controller.")]
        public CommandType FireCommand;

        [Tooltip("Sets the command key that triggers the firing if command type is set to Button type.")]
        public KeyCode CommandKey = KeyCode.Space;

        [Range(2, 99999)]
        [Tooltip("Sets the duration that shot is held when triggered and command type is set to AutoHold type.")]
        public float AutoHoldDuration = 45;

        [Tooltip("Triggers shot firing when command type is set to Automatic type.")]
        public bool TriggerAutoFire = false;

        [Range(1, 600)]
        [Tooltip("Delays TriggerAutoFire on Start by n frames when set in inspector.")]
        public int TriggerDelay = 10;


        [Header("Emitter Points")]

        [Range(0, 40)]
        public int EmitterAmount = 0;

        [Tooltip("Sets default emitter type (bullet or laser) when adding mew emitters.")]
        public PrefabType DefaultEmitter;
        protected List<GameObject> Emitters;
        protected List<GameObject> EmittersCached;
        protected List<FireBase> FireScripts;

        [Tooltip("Sets the physics/sprite layers that shots triggered by this controller belong to.")]
        public SortLayerName sortLayer;
        private SortLayerName prevLayer;
        private static int sortOrder = 9999;

        [SerializeField]
        [Range(0.1f, 1f)]
        protected float PointScale = 0.2f;

        public PointDisplayType pointDisplay = PointDisplayType.Always;

        [Header("Emission Pattern")]

        [Range(-80, 80)]
        [Tooltip("Sets the shot exit point for all child emitters.")]
        public float ExitPointOffset = 1f;

        [Range(-360, 360)]
        [Tooltip("Sets rotation for the top-level parent.")]
        public float ParentRotation;

        [Range(-180, 180)]
        [Tooltip("Sets rotation for all child emitters.")]
        public float Pitch;

        [Tooltip("Sets pitch as uni-directional or bi-directional rotation.")]
        public bool UniDirectionPitch;

        [Range(-360, 360)]
        [Tooltip("Sets the degrees of separation between child emitters.")]
        public float SpreadDegrees;

        [Range(-20, 20)]
        [Tooltip("Sets the central radius between child emitters")]
        public float SpreadRadius;

        [Tooltip("Keeps emitters fixed to anchor point regardless of radius size.")]
        public bool AutoCompRadius;

        [SerializeField]
        [Tooltip("Sets radius relative to sprite in top-level parent, if present.")]
        protected bool AutoRadiusToSprite;

        [Range(-360, 360)]
        [Tooltip("Sets central rotation for emitter group.")]
        public float CenterRotation;

        [Tooltip("Automatically centers emitter group.")]
        public bool autoCenter = true;

        protected BasePattern[] childControllers;
        protected FireBase parentPoint;

        public void Start()
        {
            initSlaveTriggers();
            linkEmittersAtLaunch();
            nestedControllerCheck();
            setIndicatorDisplay(true);

            if (!Utilities.IsEditorMode())
            {
                if (TriggerAutoFire)
                {
                    TriggerAutoFire = false;
                    StartCoroutine(CoroutineExt.WaitForFramesDo(TriggerDelay, () => TriggerAutoFire = true));
                }
            }
        }

        public virtual void LateUpdate()
        {
            setEmitters();          
            setOriginPoint();
            setIndicatorDisplay(false);
            checkSortLayerChanged();
        }

        private void initSlaveTriggers()
        {
            if (!Master || Utilities.IsEditorMode()) return;

            childControllers = GetComponentsInChildren<BasePattern>();

            for (int i = 1; i < childControllers.Length; i++)
            {
                childControllers[i].FireCommand = FireCommand;
                childControllers[i].CommandKey = CommandKey;
                childControllers[i].AutoHoldDuration = AutoHoldDuration;
            }
        }

        private void nestedControllerCheck()
        {
            var parentPointCheck = transform.parent.parent.GetComponent<FireBase>();

            if (parentPointCheck != null)
                parentPoint = parentPointCheck;
        }

        public void setEmitters()
        {
            if (Emitters != null && Emitters.Count == EmitterAmount)
                return;

            Action addNewEmitter = () => {
                GameObject p = Resources.Load<GameObject>("ND_VariaBullet/EmitterPrefabs/" + DefaultEmitter.ToString() + "/Emitter");
                GameObject newEmitter = Instantiate(p);
                newEmitter.transform.parent = this.transform;
                newEmitter.transform.localPosition = new Vector2(0, 0);
                newEmitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = this.sortLayer.ToString();
                newEmitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = sortOrder += 10;
                Emitters.Add(newEmitter);

                FireScripts.Add(newEmitter.transform.GetChild(0).GetComponent<FireBase>());
            };
            Action<string, bool, GameObject, List<GameObject>, List<GameObject>> manageCache = (name, isActive, emit, emitList1, emitList2) => {
                emit.name = name;
                emit.SetActive(isActive);
                emitList1.Add(emit);
                emitList2.Remove(emit);
            };

            if (Emitters == null)
            {
                Emitters = new List<GameObject>();
                EmittersCached = new List<GameObject>();
                FireScripts = new List<FireBase>();

                for (int i = 0; i < EmitterAmount; i++)
                    addNewEmitter();

                return;
            }

            int difference;
            int totalEmitters = Emitters.Count + EmittersCached.Count;

            if (EmitterAmount < Emitters.Count)
            {
                difference = Emitters.Count - EmitterAmount;

                for (int i = 0; i < difference; i++)
                {
                    GameObject lastEmitter = Emitters[Emitters.Count - 1];
                    manageCache("Emitter(Cached)", false, lastEmitter, EmittersCached, Emitters);
                }
            }
            else if (EmitterAmount > Emitters.Count)
            {
                difference = EmitterAmount - totalEmitters;

                if (EmitterAmount > totalEmitters)
                {                                        
                    foreach (GameObject emitterStored in EmittersCached.ToArray()) //ToArray fix to ensure collection items aren't removed as it's being iterated over.
                        manageCache("Emitter(Clone)", true, emitterStored, Emitters, EmittersCached);

                    for (int i = 0; i < difference; i++)
                        addNewEmitter();
                }
                else
                {
                    difference = EmitterAmount - Emitters.Count;

                    for (int i = 0; i < difference; i++)
                    {
                        GameObject lastEmitter = EmittersCached[EmittersCached.Count - 1];
                        manageCache("Emitter(Clone)", true, lastEmitter, Emitters, EmittersCached);
                    }
                }
            }
        }

        private void linkEmittersAtLaunch() //for re-establishing emitter list on project startup
        {
            if (Emitters != null)
                return;

            if (transform.childCount > 0)
            {
                Emitters = new List<GameObject>();
                EmittersCached = new List<GameObject>();
                FireScripts = new List<FireBase>();

                foreach (Transform child in this.transform)
                {
                    if (child.parent == this.transform)
                    {
                        FireScripts.Add(child.transform.GetChild(0).GetComponent<FireBase>());

                        if (child.gameObject.activeSelf)
                            Emitters.Add(child.gameObject);
                        else
                            EmittersCached.Add(child.gameObject);
                    }
                }
            }
        }

        private void setOriginPoint()
        {
            if (origin == prevOrigin || origin == AutoSetOrigin.Manual)
                return;

            Transform originPoint = transform.parent;
            Transform mainSource = originPoint.parent;
            SpriteRenderer mainRend = mainSource.GetComponent<SpriteRenderer>();

            if (mainSource == null || mainRend == null || mainRend.sprite == null)
                return;

            float x = 0;

            if (origin == AutoSetOrigin.Tip)
                x = mainSource.GetComponent<SpriteRenderer>().sprite.bounds.center.x + mainSource.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 2;
            else
                x = mainSource.GetComponent<SpriteRenderer>().sprite.bounds.center.x;

            originPoint.localPosition = new Vector2(x, 0);
            prevOrigin = origin;
        }

        private void setIndicatorDisplay(bool forceOnStart)
        {
            if (Utilities.IsEditorMode() || forceOnStart)
            {
                bool display = false;

                switch (pointDisplay)
                {
                    case PointDisplayType.Always: display = true;
                        break;
                    case PointDisplayType.Never: display = false;
                        break;
                    case PointDisplayType.EditorOnly: display = (forceOnStart) ? false : true;
                        break;
                }

                foreach (Transform child in transform)
                {
                    Transform point = child.GetChild(0);
                    if (point.name == "Point")
                        point.GetComponent<SpriteRenderer>().enabled = display;
                }
            }
        }

        private void checkSortLayerChanged()
        {
            if (sortLayer != prevLayer)
            {
                foreach (GameObject emitter in Emitters)
                    emitter.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = this.sortLayer.ToString();
            }

            prevLayer = sortLayer;
        }

        public void clearEmitterCache()
        {
            if (EmittersCached.Count == 0) { Utilities.Warn("No cached Emitters found.", this, transform.parent.parent); return; }

            foreach (GameObject emitter in EmittersCached)
            {
                FireScripts.Remove(emitter.transform.GetChild(0).GetComponent<FireBase>());
                DestroyImmediate(emitter.gameObject);
            }

            EmittersCached = new List<GameObject>();
            sortOrder = 9999;
        }

        public void cloneFirstEmitter()
        {
            if (EmitterAmount <= 1) { Utilities.Warn("Not enough Emitters to clone to (2 or more required).", this, transform.parent.parent); return; }
            if (FreezeEdits) { Utilities.Warn("Cannot clone while edits are frozen. Disable FreezeEdits temporarily in order to use clone procedure.", this, transform.parent.parent); return; }

            Action<FireBase> copyProcess = (FireBase src) =>
            {
                for (int i = 1; i < Emitters.Count; i++)
                {
                    FireBase targ = Emitters[i].transform.GetChild(0).gameObject.GetComponent<FireBase>();

                    targ.customIndicator = src.customIndicator;
                    targ.Shot = src.Shot;
                    targ.ShotSpeed = src.ShotSpeed;
                    targ.LocalOffset = src.LocalOffset;
                    targ.makeNodeOnly = src.makeNodeOnly;
                    targ.SpriteColor = src.SpriteColor;
                    targ.LocalPitch = src.LocalPitch;

                    if (src.GetType() == typeof(FireBullet)) //extended copying in case of FireBullet type emitter
                    {
                        FireBullet srcXT = src as FireBullet;
                        FireBullet targXT = Emitters[i].transform.GetChild(0).gameObject.GetComponent<FireBullet>();

                        targXT.SpriteOverride = srcXT.SpriteOverride;
                        targXT.ParentToEmitter = srcXT.ParentToEmitter;
                        targXT.IgnoreGlobalRateScale = srcXT.IgnoreGlobalRateScale;
                        targXT.ShotRate = srcXT.ShotRate;
                        targXT.PauseRate = srcXT.PauseRate;
                        targXT.PauseLength = srcXT.PauseLength;
                        targXT.ShotOverlap = srcXT.ShotOverlap;
                        targXT.PoolingEnabled = srcXT.PoolingEnabled;
                        targXT.AutoPool = srcXT.AutoPool;
                        targXT.AutoPoolOverride = srcXT.AutoPoolOverride;   
                        targXT.BankingEnabled = srcXT.BankingEnabled;
                    }
                }
            };

            var scriptToClone = Emitters[0].transform.GetChild(0).gameObject.GetComponent<FireBase>();

            if (DefaultEmitter == PrefabType.Bullet)
                copyProcess(scriptToClone as FireBullet);    
            else
                copyProcess(scriptToClone as FireExpanding);
        }

        public void ResetEmitters()
        {
            foreach (GameObject emitter in Emitters)
                Destroy(emitter);

            foreach (GameObject emitterCached in EmittersCached)
                Destroy(emitterCached);

            Emitters = new List<GameObject>();
            EmittersCached = new List<GameObject>();
            FireScripts = new List<FireBase>();

            EmitterAmount = 0;
        }

        public void addAutomator(string type)
        {
            if (type == "Linear")
                gameObject.AddComponent<AutomateLinear>();
            else
                gameObject.AddComponent<AutomateStepped>();
        }

        public enum AutoSetOrigin
        {
            Manual,
            Tip,
            Center
        }

        public enum PatternSelect
        {
            Stack,
            Radial
        }

        public enum CommandType
        {
            ButtonPress = 0,
            ButtonPressAutoHold,
            Automatic,
            AutomaticAutoHold
        }

        public enum SortLayerName
        {
            ND_PlayerBullet,
            ND_EnemyBullet,
            ND_SelfColliding,
            ND_UserBullet1,
            ND_UserBullet2,
            ND_UserBullet3,
            ND_UserBullet4,
            ND_UserBullet5,
            ND_UserBullet6,
            ND_UserBullet7,
            ND_UserBullet8,
            Player,
            Enemy
        }

        public enum PrefabType
        {
            Bullet,
            Expanding
        }

        public enum PointDisplayType
        {
            Always,
            Never,
            EditorOnly
        }

        delegate void Action<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }
}
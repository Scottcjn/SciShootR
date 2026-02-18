#region Script Synopsis
    //Base class for firing scripts attached to emitters. Mainly used for common firing properties.
    //Examples: FireBullet, FireExpanding (lasers)
    //Learn more about firing scripts at: https://neondagger.com/variabullet2d-quick-start-guide/#firing-shots
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    [ExecuteInEditMode]
    public abstract class FireBase : MonoBehaviour
    {
        [Tooltip("Changes from emitter behavior (firing) to a node only (non-firing).")]
        public bool makeNodeOnly;

        [Range(-180, 180)]
        [Tooltip("Sets local rotation for this emitter.")]
        public float LocalPitch;

        [Tooltip("Sets image replacement for point indicator.")]
        public Sprite customIndicator;

        [Tooltip("Prefab shot type that this emitter fires.")]
        public GameObject Shot;
        protected SpriteRenderer rend;

        protected const float speedLimit = 100;

        [Tooltip("Sets the shot's speed when fired.")]
        [Range(.01f, speedLimit)]
        public float ShotSpeed = 30;

        [Range(-40, 40)]
        [Tooltip("Sets the local shot exit point relative to this emitter.")]
        public float LocalOffset = 0;

        [HideInInspector]
        public BasePattern controller;

        private Func<bool>[] fireMethods = new Func<bool>[4];

        protected bool AutoHold = false;
        protected Timer AutoHoldCounter = new Timer(0);

        [HideInInspector]
        public UnityEngine.Events.UnityEvent OnStoppedFiring;

        [Tooltip("Overrides the fired shot's color.")]
        public Color SpriteColor;

        protected AudioSource audiosrc;

        public virtual void Start()
        {
            controller = transform.parent.parent.GetComponent<BasePattern>();

            fireMethods[0] = ButtonPress;
            fireMethods[1] = ButtonPressAutoHold;
            fireMethods[2] = AutoFire;
            fireMethods[3] = AutoFireAutoHold;

            rend = GetComponent<SpriteRenderer>();
            audiosrc = GetComponent<AudioSource>();
        }

        public virtual void LateUpdate()
        {
            if (isNode()) return;
            if (Utilities.IsEditorMode()) return;

            bool fireCommanded = fireMethods[(int)controller.FireCommand]();

            if (fireCommanded && ShootAtCurrentInterval())
                InstantiateShot();

            if (!fireCommanded)
                if (OnStoppedFiring != null)
                    OnStoppedFiring.Invoke();
        }

        public void ConnectAudio()
        {
            if (GetComponent<AudioSource>() == null)
                gameObject.AddComponent<AudioSource>().playOnAwake = false;
            else
                Debug.Log("Audio Event Already Attached");
        }

        protected abstract bool ButtonPress();
        protected virtual bool ButtonPressAutoHold()
        {
            return AutoHoldTemplate(Input.GetKeyDown(controller.CommandKey));
        }
        protected abstract bool AutoFire();
        protected virtual bool AutoFireAutoHold()
        {
            return AutoHoldTemplate(controller.TriggerAutoFire);
        }
        protected abstract bool AutoHoldTemplate(bool commandType);
        protected abstract bool ShootAtCurrentInterval();

        public abstract void InstantiateShot();

        private bool isNode()
        {
            if (Utilities.IsEditorMode())
            {
                var indicatorRend = GetComponent<SpriteRenderer>();
                Sprite pointIndicator;
                Sprite nodeIndicator = Resources.Load<Sprite>("ND_VariaBullet/PointSprites/NodeIndicator");

                if (customIndicator == null)
                {
                    pointIndicator = Resources.Load<Sprite>("ND_VariaBullet/PointSprites/ArrowIndicator");
                    indicatorRend.color = new Color(indicatorRend.color.r, indicatorRend.color.b, indicatorRend.color.g, .51f);
                }
                else
                {
                    pointIndicator = customIndicator;
                    indicatorRend.color = new Color(indicatorRend.color.r, indicatorRend.color.b, indicatorRend.color.g, 1);
                }

                indicatorRend.sprite = makeNodeOnly ? nodeIndicator : pointIndicator;
            }

            return makeNodeOnly;
        }

        protected void checkShotMismatch(Type conflicting, string expected)
        {
            var shotScript = Shot.GetComponent<ShotBase>();

            if (shotScript == null)
                Utilities.Warn("FireBullet script doesn't have a valid Shot " + expected + " prefab. " + Shot.name + " is not a valid Shot type.", this, transform.root);
            else
            {
                if (shotScript.GetType() == conflicting || shotScript.GetType().IsSubclassOf(conflicting))
                    Utilities.Warn("Shot prefab " + Shot.name + " is not a valid " + expected + " prefab.", this, transform.root);
            }
        }
    }
}
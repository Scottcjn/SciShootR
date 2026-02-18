#region Script Synopsis
    //Subclass of FireBase for instantiating lasers and laser-like projects. A Monobehavior that comes attached to an emitter point gameobject.
    //Instantiation can occur at different intervals depending on methods chosen.
    //Learn more about firing scripts at: https://neondagger.com/variabullet2d-quick-start-guide/#firing-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class FireExpanding : FireBase
    {
        private GameObject shotRef;

        public override void Start()
        {
            if (Utilities.IsEditorMode())
            {
                if (!makeNodeOnly)
                    checkShotMismatch(typeof(ShotBullet), "Laser");

                return;
            }

            base.Start();
        }

        public override void InstantiateShot()
        {        
            GameObject firedShot = Instantiate(Shot) as GameObject;
            firedShot.transform.parent = this.transform;

            ShotBase shotScript = firedShot.GetComponent<ShotBase>();
            shotScript.ShotSpeed = ShotSpeed;
            shotScript.ExitPoint = controller.ExitPointOffset + LocalOffset;
            shotScript.Emitter = this.transform;
            shotScript.FiringScript = this;

            int physicsLayer = LayerMask.NameToLayer(rend.sortingLayerName);
            firedShot.layer = physicsLayer;

            shotScript.sortLayer = rend.sortingLayerName;
            shotScript.sortOrder = rend.sortingOrder - 9999;
            shotScript.InitialSet();

            shotRef = firedShot;

            if (audiosrc != null)
            {
                audiosrc.mute = false;
                audiosrc.loop = true;
                audiosrc.Play();
               
                OnStoppedFiring.AddListener(muteAudio);
            }
        }

        private void muteAudio()
        {
            audiosrc.loop = false;
            audiosrc.mute = true;
            OnStoppedFiring.RemoveListener(muteAudio);
        }

        protected override bool ButtonPress()
        {
            if (Input.GetKeyDown(controller.CommandKey))
                return false;

            return Input.GetKey(controller.CommandKey);
        }

        protected override bool ButtonPressAutoHold()
        {
            return base.ButtonPressAutoHold();
        }

        protected override bool AutoFire()
        {
            return controller.TriggerAutoFire;
        }

        protected override bool AutoFireAutoHold()
        {
            return base.AutoFireAutoHold();
        }

        protected override bool AutoHoldTemplate(bool commandType)
        {
            if (commandType && !AutoHold)
                AutoHold = true;

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
            if (shotRef == null)
                return true;
            else
            {
                if (shotRef.transform.parent.name == "Packet")
                    return true;

                return false;
            }
        }
    }
}
#region Script Synopsis
    //A bullet shot type that contains its own controller/emitters.
    //Either continously fires its own shots or shoots once when "shootonce" is enabled.
    //Learn more on this custom shot type at: https://neondagger.com/variabullet2d-scripting-guide/#shots-with-emitters
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotEmitPattern : ShotNonPhysics
    {
        [Header("Emission Setting")]

        [Tooltip("Ensures that child controller fires on only one frame.")]
        public bool shootOnce = true;
        private BasePattern childController;

        public override void Start()
        {
            base.Start();
            childController = transform.GetChild(0).GetChild(0).GetComponent<BasePattern>();
            childController.TriggerAutoFire = true;
        }

        public override void Update()
        {
            base.Update();

            if (shootOnce)
                childController.TriggerAutoFire = false;
        }

        protected override void setSprite(SpriteRenderer sr)
        {
            FireBullet fireScript = this.FiringScript as FireBullet;
            if (fireScript.SpriteOverride == null)
                return;

            Transform child = transform.GetChild(0).GetChild(0);

            foreach (Transform emitter in child)
            {
                foreach (Transform point in emitter)
                {
                    FireBullet pointScript = point.GetComponent<FireBullet>();
                    pointScript.SpriteOverride = fireScript.SpriteOverride;
                }
            }
        }
    }
}
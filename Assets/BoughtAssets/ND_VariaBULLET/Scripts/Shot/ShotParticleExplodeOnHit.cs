#region Script Synopsis
    //A shot that has it's own controller/emitters, which fire upon collision, triggering a particle effect
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotParticleExplodeOnHit : ShotNonPhysics, IRePoolable
    {
        [Header("Collision Override")]
        [Tooltip("Bypasses CollisionList filtering, instead registering all collisions detected by the underlying collision matrix.")]
        public bool IgnoreCollisionFilter;

        private int frameBuffer = 3;
        private BasePattern childController;
        private bool hitDetected;

        public override void InitialSet()
        {
            base.InitialSet();

            childController = transform.GetChild(0).GetChild(0).GetComponent<BasePattern>();
        }

        public override void Update()
        {
            base.Update();

            if (hitDetected)
            {
                frameBuffer--;

                if (frameBuffer <= 0)
                    RePoolOrDestroy();
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (IgnoreCollisionFilter || CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                childController.TriggerAutoFire = true;
                rend.enabled = false;
                hitDetected = true;
            }
        }

        public override void RePool(IPooler poolingScript)
        {
            childController.TriggerAutoFire = false;
            hitDetected = false;
            frameBuffer = 3;
            rend.enabled = true;
            base.RePool(poolingScript);
        }
    }
}
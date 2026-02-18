#region Script Synopsis
    //A bullet shot that allows for dynamic enabling/disabling of collider for performance purposes.
    //References a single static object set via GlobalShotManager.StaticColl prefab.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotCollFlux : ShotBase
    {
        [Tooltip("Allows dynamic enabling/disabling of collider as performance gain. [NOTE: requires a static collision object set via GlobalShotManager.StaticColl]")]
        public bool ColliderFlux;
        private Collider2D coll;

        public override void InitialSet()
        {
            if (ColliderFlux)
            {
                coll = GetComponent<Collider2D>();
                coll.enabled = false;
            }

            base.InitialSet();
        }

        public override void Update()
        {
            collFluxCheck();
            base.Update();
        }

        private void collFluxCheck()
        {
            if (GlobalShotManager.Instance.StaticCollTransform == null || !ColliderFlux)
                return;

            if (Vector2.Distance(this.transform.position, GlobalShotManager.Instance.StaticCollTransform.position) < GlobalShotManager.Instance.StaticCollRadius)
                coll.enabled = true;
            else
                coll.enabled = false;
        }
    }
}
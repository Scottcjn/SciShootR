#region Script Synopsis
    //Subclass used for animating rear blast on a missle.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class BlastMissile : BlastAnimBase
    {
        private ShotHomingInertial shotScript;

        protected override void Start()
        {
            shotScript = transform.parent.GetComponent<ShotHomingInertial>();
            var rend = GetComponent<SpriteRenderer>();
            rend.sortingLayerName = shotScript.sortLayer;
            rend.sortingOrder = shotScript.sortOrder - 1;
            base.Start();
        }

        protected override void Update()
        {
            trigger = shotScript.burstFlag;
            base.Update();
        }
    }
}
#region Script Synopsis
    //The base class for all linear trajectory physics type shots that are not re-poolable.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShotPhysics : ShotBaseColorizable
    {
        protected Rigidbody2D body;

        public override void InitialSet()
        {
            base.InitialSet();
            movement();
        }

        protected virtual void movement()
        { 
            //No implementation by default
        }
    }
}
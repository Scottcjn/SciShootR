#region Script Synopsis
    //The base class for all linear trajectory physics type shots that are re-poolable.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotLinearPhysics : ShotPhysics, IRePoolable
    {
        protected override void movement()
        {      
            body = GetComponent<Rigidbody2D>();
            setVelocity(ShotSpeed);
        }

        public virtual void FixedUpdate()
        {
            scaledSpeed = ShotSpeed * scale;
            setVelocity(scaledSpeed);
        }

        private void setVelocity(float speed)
        {
            body.velocity = new Vector2(
                speed * Trajectory.x,
                speed * Trajectory.y
            );
        }
    }
}
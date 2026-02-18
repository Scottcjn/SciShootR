#region Script Synopsis
    //A physics type shot that uses a "bouncy" material on the collider to create deflection off of collidable objects.
    //Doesn't become destroyed through the conventional OnCollisionEnter2D() method.
    //Examples: used in "PuzzleShooter" demo.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBouncyPhysics : ShotLinearPhysics
    {
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            //No implementation. Implement if object should be destroyed on collision or other behavior required
        }

        public override void FixedUpdate()
        {
            //No Implementation. Motion set in ShotPhysics.InitialSet()
        }
    }
}
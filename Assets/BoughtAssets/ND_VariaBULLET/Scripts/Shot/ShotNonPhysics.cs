#region Script Synopsis
    //The base class for all non-physics type shots that are not re-poolable.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotNonPhysics : ShotBaseColorizable
    {
        private Vector2 move;

        public override void Update()
        {
            base.Update();
            movement();
        }

        private void movement()
        {
            move.x = scaledSpeed * Time.deltaTime * Trajectory.x;
            move.y = scaledSpeed * Time.deltaTime * Trajectory.y;

            transform.position += new Vector3(move.x, move.y, 0);
        }
    }
}
#region Script Synopsis
    //The interface IShotCollidable is used to represent any kind of object which can collide with a bullet or laser.
    //Examples: ShotCollision, ShotCollisionDamage

    //CollisionArgs is used to transmit sender information in a single object over to the object being collided with.
    //Examples: ShotCollisionDamage.OnLaserCollision(), ShotCollision.OnLaserCollision()
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public interface IShotCollidable
    {
        IEnumerator OnLaserCollision(CollisionArgs sender);
        IEnumerator OnCollisionEnter2D(Collision2D collision);
    }

    public class CollisionArgs
    {
        public GameObject gameObject;
        public Vector2 point;
        public float damage;

        public CollisionArgs(GameObject gameObject, Vector2 point, float damage)
        {
            this.gameObject = gameObject;
            this.point = point;
            this.damage = damage;
        }
    }
}
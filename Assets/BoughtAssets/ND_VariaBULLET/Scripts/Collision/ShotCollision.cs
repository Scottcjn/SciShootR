#region Script Synopsis
    //A monobehavior that is attached to any object that receives collisions from bullet/laser shots and instantiates explosions if set.
    //Examples: Any non-damaging solid object such as terrain, walls, platforms and so forth.
    //Learn more about the collision system at: https://neondagger.com/variabullet2d-system-guide/#collision-system
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public class ShotCollision : MonoBehaviour, IShotCollidable
    {
        [Tooltip("Sets which collision layers can produce explosions when colliding with this object.")]
        public string[] CollisionList;

        [Tooltip("Sets the name of the explosion prefab that's instantiated upon laser collision. [Note: prefab must also be pre-loaded in GlobalShotManager.ExplosionPrefabs].")]
        public string LaserExplosion;

        [Tooltip("Sets the name of the explosion prefab that's instantiated upon bullet collision. [Note: prefab must also be pre-loaded in GlobalShotManager.ExplosionPrefabs].")]
        public string BulletExplosion;

        [Tooltip("Sets whether or not explosion moves with this object or remains at point of impact.")]
        public bool ParentExplosion = true;

        public IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            if (CollisionFilter.collisionAccepted(sender.gameObject.layer, CollisionList))
            {
                CollisionFilter.setExplosion(LaserExplosion, ParentExplosion, this.transform, new Vector2(sender.point.x, sender.point.y), 0, this);
                yield return null;
            }
        }

        public IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                CollisionFilter.setExplosion(BulletExplosion, ParentExplosion, this.transform, collision.contacts[0].point, 0, this);
                yield return null;
            }
        }
    }
}
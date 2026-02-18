#region Script Synopsis
    //A subclass of ShotBase, extending basic functionality used by all bullets.
    //Functionality includes bullets having their own explosions, out-of-bounds behavior and setting their current x/y direction.
    //Learn more about bullet type shots at: https://neondagger.com/variabullet2d-in-depth-shot-guide/#bullet-shots
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBullet : ShotCollFlux
    {       
        [Header("Collision Settings")]

        [Tooltip("Sets which collision layers can produce explosions when this shot collides with them.")]
        public string[] CollisionList;

        [Tooltip("Sets the name of the explosion prefab that's instantiated upon collision. [Note: must also be pre-loaded in GlobalShotManager.ExplosionPrefabs].")]
        public string Explosion;

        [Tooltip("Sets whether or not explosion moves with the object it collided with or remains at point of impact.")]
        public bool ParentExplosion;

        [Tooltip("Sets rotation of explosion relative to the angle the shot was travelling.")]
        public bool ExplosionRotation;

        [HideInInspector]
        public Vector2 lastPosition;

        [HideInInspector]
        public Vector2 newPosition;

        [HideInInspector]
        public Vector2 Direction;

        public float AngleDirection {
            get
            {
                return (!ExplosionRotation) ? 0 : CalcObject.AngleBetweenVectors(lastPosition, newPosition);
            }
        }

        public override void Update()
        {
            lastPosition = transform.position;
            base.Update();
        }

        public virtual void LateUpdate()
        {
            //Necessary to run in LateUpdate for producing accurate directional tracking
            newPosition = transform.position;
            Direction = setDirection(lastPosition, newPosition);
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            
            if (CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                CollisionFilter.setExplosion(Explosion, ParentExplosion, collision.gameObject.transform, collision.contacts[0].point, AngleDirection, this);
            }
        }

        private Vector2 setDirection(Vector2 lastPos, Vector2 newPos)
        {
            float x = (newPos.x - lastPos.x >= 0) ? 1 : -1;
            float y = (newPos.y - lastPos.y >= 0) ? 1 : -1;

            return new Vector2(x,y);
        }
    }
}
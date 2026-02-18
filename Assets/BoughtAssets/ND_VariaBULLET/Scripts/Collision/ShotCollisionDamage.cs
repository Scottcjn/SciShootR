#region Script Synopsis
    //A monobehavior that is attached to any object that receives collisions from bullet/laser shots and instantiates explosions if set and applies damage to the object.
    //Examples: Any object that receives damage (player, enemy, etc).
    //Learn more about the collision system at: https://neondagger.com/variabullet2d-system-guide/#collision-system
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public class ShotCollisionDamage : ShotCollision, IShotCollidable
    {
        [Tooltip("Sets the name of the explosion prefab to be instantiated when HP = 0.")]
        public string DeathExplosion;

        [Tooltip("Health Points (if object is an enemy). Reduces according to incoming IDamager.DMG value upon collision.")]
        public float enemyHP = 10;

        [Range(0.1f, 8f)]
        [Tooltip("Changes the size of the last explosion (when HP = 0).")]
        public float FinalExplodeFactor = 2;

        [Tooltip("Enables indicating damage by flickering color (via DamageColor setting) when HP is reducing.")]
        public bool DamageFlicker;

        [Range(5, 40)]
        [Tooltip("Sets the duration frames for the DamageFlicker effect upon collision.")]
        public int FlickerDuration = 6;

        [Tooltip("Sets the color the object flickers to when HP is reducing and DamageFlicker is enabled.")]
        public Color DamageColor;
        private Color NormalColor;
        private SpriteRenderer rend;

        void Start()
        {
            rend = GetComponent<SpriteRenderer>();
            NormalColor = rend.color;
        }

        public new IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            if (CollisionFilter.collisionAccepted(sender.gameObject.layer, CollisionList))
            {
                setBulletsDamage(sender.damage);
                CollisionFilter.setExplosion(LaserExplosion, ParentExplosion, this.transform, new Vector2(sender.point.x, sender.point.y), 0, this);
                yield return setFlicker();
            }
        }

        public new IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            CheckForPlayerAndEnemyCollision(collision.gameObject);
            if (CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList))
            {
                setBulletsDamage(collision.gameObject.GetComponent<IDamager>().DMG);
                CollisionFilter.setExplosion(BulletExplosion, ParentExplosion, this.transform, collision.contacts[0].point, 0, this);
                yield return setFlicker();
            }
        }
        private void CheckForPlayerAndEnemyCollision(GameObject collision)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                float enemyDamage = 1;
                CheckForPlayerAndTakeDamage(enemyDamage);
                enemy.Die();
            }
        }
        protected void setBulletsDamage(float bulletdamage)
        {
            CheckForPlayerAndTakeDamage(bulletdamage);

            enemyHP -= bulletdamage;
            CheckForEnemyDeath();
        }
        private void CheckForPlayerAndTakeDamage(float bulletdamage)
        {
            Player player = gameObject.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage(bulletdamage);
            }
        }
        private void CheckForEnemyDeath()
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy && enemyHP <= 0)
            {
                if (DeathExplosion != "")
                {
                    string explosion = DeathExplosion;
                    GameObject finalExplode = GlobalShotManager.Instance.ExplosionRequest(explosion, this);

                    finalExplode.transform.position = this.transform.position;
                    finalExplode.transform.parent = null;
                    finalExplode.transform.localScale = new Vector2(finalExplode.transform.localScale.x * FinalExplodeFactor, finalExplode.transform.localScale.y * FinalExplodeFactor);
                }
                StartEnemyDeathFunction(enemy);
            }
        }
        private void StartEnemyDeathFunction(Enemy enemy)
        {
            if (enemy)
            {
                enemy.Die();
            }
        }

        protected IEnumerator setFlicker()
        {
            if (rend == null)
            {
                Utilities.Warn("No SpriteRenderer attached. Cannot flicker during damage.", this);
                yield return null;
            }

            if (DamageFlicker)
            {
                bool flicker = false;
                for (int i = 0; i < FlickerDuration * 2; i++)
                {
                    flicker = !flicker;

                    if (flicker)
                        rend.color = DamageColor;
                    else
                        rend.color = NormalColor;

                    yield return null;
                };

                rend.color = NormalColor;
            }
        }
    }
}
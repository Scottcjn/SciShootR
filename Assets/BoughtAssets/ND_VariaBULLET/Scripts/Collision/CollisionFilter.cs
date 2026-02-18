#region Script Synopsis
    //These static methods are called by any script which needs to filter collidable objects and set the requisite explosions.
    //Examples: ShotBase.OnCollisionEnter2D(), ShotCollision.OnCollisionEnter2D(), ShotCollisionDamage.OnCollisionEnter2D()
    //Learn more about explosions at: https://neondagger.com/variabullet2d-system-guide/#explosions
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public static class CollisionFilter
    {
        public static bool collisionAccepted(int layer, string[] collisionList)
        {
            if (collisionList.Length == 0)
                return false;

            foreach (string item in collisionList)
                if (LayerMask.LayerToName(layer) == item)
                    return true;

            return false;
        }

        public static void setExplosion(string type, bool parentExplosion, Transform parent, Vector2 position, float rotation, object sender)
        {
            if (type == "")
                return;

            GameObject explosion = GlobalShotManager.Instance.ExplosionRequest(type, sender);
            float rot = (rotation == 0) ? explosion.transform.eulerAngles.z : rotation;
            explosion.transform.rotation = Quaternion.Euler(0, 0, rot);
            explosion.transform.position = position;

            if (parentExplosion)
                explosion.transform.parent = parent;
        }
    }
}
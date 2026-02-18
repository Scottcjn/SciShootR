#region Script Synopsis
    //A special case script for applying damage from one body to another during collision.
    //Attach to body of player or enemy in order to transmit collision damage information during collision.
    //Learn more about the collision system at: https://neondagger.com/variabullet2d-system-guide/#collision-system
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class DamagerBody : MonoBehaviour, IDamager
    {
        [Tooltip("Sets the amount of HP reduction produced by this object when collides with a ShotCollidable object.")]
        public float DamagePerHit;
        public float DMG { get { return DamagePerHit; } }
    }
}
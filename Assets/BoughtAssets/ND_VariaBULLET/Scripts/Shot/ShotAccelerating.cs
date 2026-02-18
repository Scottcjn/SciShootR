#region Script Synopsis
    //A shot script attached to a non-physics bullet prefab, propelling the shot at an accelerating pace.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotAccelerating : ShotNonPhysics, IRePoolable
    {
        [Header("Acceleration Settings")]

        [Tooltip("Sets the rate at which acceleration occurs.")]
        public AccelType VelocityCurve;

        [Range(0.01f, 0.5f)]
        [Tooltip("Sets the base rate of shot acceleration. [Lower number = slower acceleration].")]
        public float AccelFactor;

        [Range(0,100)]
        [Tooltip("Sets the highest speed limit that a shot can reach before it maintains a constant speed.")]
        public float SpeedLimit;

        private float speedOriginal;

        public override void InitialSet()
        {
            base.InitialSet();
            speedOriginal = ShotSpeed;
        }

        public override void Update()
        {
            if (VelocityCurve == AccelType.exponential)
            {
                float exponential = AccelFactor * scale + 1;
                ShotSpeed *= exponential;
            }
            else
            {
                float linear = AccelFactor * speedOriginal * scale;
                ShotSpeed += linear;
            }

            if (SpeedLimit != 0)        
                if (ShotSpeed >= SpeedLimit)
                    ShotSpeed = SpeedLimit;

            base.Update();
        }

        public enum AccelType
        {
            linear,
            exponential
        }
    }
}
#region Script Synopsis
    //A physics based bullet shot which adds to its speed at "burst" intervals
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBurstPhysics : ShotPhysics, IRePoolable
    {
        [Header("Burst Settings")]

        [Range(100, 1)]
        [Tooltip("Sets the time interval between propelling bursts of force. [Higher number = less bursts].")]
        public int BurstFrequency;
        private int prevFreqeuncy;

        [Range(0, 1)]
        [Tooltip("Scales down the initial propelling burst of force. [0 = stand still; 1 = normal shot speed].")]
        public float InitialBurst;

        private Timer timer;

        protected override void movement()
        {
            timer = new Timer(0);
            body = GetComponent<Rigidbody2D>();
            body.velocity = new Vector2(ShotSpeed * InitialBurst * Trajectory.x, ShotSpeed * InitialBurst * Trajectory.y);
        }

        public override void Update()
        {
            base.Update();

            if (BurstFrequency != prevFreqeuncy)
                timer.Reset();
            
            timer.Run(BurstFrequency);

            if (timer.Flag)
            {
                body.AddForce(
                    new Vector2(scaledSpeed / 10 * Trajectory.x, scaledSpeed / 10 * Trajectory.y),
                    ForceMode2D.Impulse
                );

                timer.Reset();
            }

            prevFreqeuncy = BurstFrequency;          
        }
    }
}
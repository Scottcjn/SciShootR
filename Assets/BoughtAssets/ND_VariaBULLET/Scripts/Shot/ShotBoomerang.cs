#region Script Synopsis
    //A non-physics Bullet which boomerangs back to its original emission point.
    //Action and description of fields can be found at https://neondagger.com/variabullet2d-in-depth-shot-guide/#default-shot-prefabs
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBoomerang : ShotBaseColorizable, IRePoolable
    {
        [Header("Boomerang Settings")]

        [Range(0.1f, 3f)]
        [Tooltip("Sets the speed factor at which the shot returns. [0.5 = half speed; 1 = no change. 2 = double speed.")]
        public float ReturnSpeed = 1.2f;

        [Range(0.5f, 100)]
        [Tooltip("Sets the range in time units at which the shot returns.")]
        public float returnLimit;
        private Timer returnTimer = new Timer(0);

        [Tooltip("Automatically adjusts ReturnLimit. Resulting in always returning back at the same distance travelled regardless of shot speed.")]
        public bool ReturnLimitAutoScale;

        private Vector2 move;

        public override void InitialSet()
        {
            base.InitialSet();
            returnTimer.Reset();
        }

        public override void Update()
        {
            base.Update();

            if (!returnTimer.Flag)
                shotSend();
            else
                shotReturn();
        }

        private void shotSend()
        {
            move.x = scaledSpeed * Time.deltaTime * Trajectory.x;
            move.y = scaledSpeed * Time.deltaTime * Trajectory.y;

            transform.position += new Vector3(move.x, move.y, 0);
            checkReturn();
        }

        private void shotReturn()
        {
            transform.position = Vector3.MoveTowards(transform.position, Emitter.transform.position, Time.deltaTime * scaledSpeed * ReturnSpeed);
            checkReturned();
        }

        private void checkReturn()
        {
            float returnScale;

            if (ReturnLimitAutoScale)
                returnScale = returnLimit / scaledSpeed;
            else
                returnScale = 1;

            returnTimer.Run(returnLimit * returnScale);
        }

        private void checkReturned()
        {
            if (Vector3.Distance(this.transform.position, Emitter.transform.position) < 0.1f)
                RePoolOrDestroy();
        }
    }
}
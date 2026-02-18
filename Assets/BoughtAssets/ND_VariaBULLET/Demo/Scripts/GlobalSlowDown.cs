#region Script Synopsis
    //A demonstration script for controlling global shot speed in real-time. Used in the "HorizontalShooter" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class GlobalSlowDown : MonoBehaviour
    {
        public KeyCode button;
        private bool toggle;

        [Range(0.01f, 0.9f)]
        public float SlowestSpeed;
        private float normalSpeed;

        [Range(1, 10)]
        public float TransitionSpeed = 6;

        void Start()
        {
            normalSpeed = GlobalShotManager.Instance.SpeedScale;
        }

        void Update()
        {
            if (Input.GetKeyDown(button))
                toggle = !toggle;

            if (toggle)
            {
                GlobalShotManager.Instance.SpeedScale = Mathf.Lerp(
                    GlobalShotManager.Instance.SpeedScale,
                    SlowestSpeed,
                    Time.deltaTime * TransitionSpeed
                );
            }
            else
            {
                GlobalShotManager.Instance.SpeedScale = Mathf.Lerp(
                    GlobalShotManager.Instance.SpeedScale,
                    normalSpeed,
                    Time.deltaTime * TransitionSpeed
                );
            }
        }
    }
}
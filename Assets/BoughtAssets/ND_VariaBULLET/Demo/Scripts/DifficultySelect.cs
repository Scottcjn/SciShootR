#region Script Synopsis
    //Simple demo script for creating a shot speed/rate based variable difficulty
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class DifficultySelect : MonoBehaviour
    {
        float upperLimit = 5;
        float lowerLimit = 1;

        float difficulty = 1;
        float increment = 0.5f;

        private void Start()
        {
            GlobalShotManager.Instance.LockRateToSpeed = true;
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                difficulty = (difficulty >= upperLimit) ? upperLimit : difficulty + increment;
                GlobalShotManager.Instance.SpeedScale = difficulty;
            }               
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                difficulty = (difficulty <= lowerLimit) ? lowerLimit : difficulty - increment;
                GlobalShotManager.Instance.SpeedScale = difficulty;
            }               
        }
    }
}
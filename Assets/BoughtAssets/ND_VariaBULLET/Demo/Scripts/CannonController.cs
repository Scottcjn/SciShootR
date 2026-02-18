#region Script Synopsis
    //Controls the custom point indicator "cannons" in the "VerticalShooter" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CannonController : MonoBehaviour
    {
        public int Speed = 1;
        public int limit;
        private BasePattern controller;

        void Start()
        {
            controller = GetComponent<BasePattern>();        
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
                controller.Pitch -= Speed;
            else if (Input.GetKey(KeyCode.DownArrow))
                controller.Pitch += Speed;

            if (controller.Pitch < limit * -1)
                controller.Pitch = limit * -1;
            else if (controller.Pitch > limit)
                controller.Pitch = limit;
        }
    }
}
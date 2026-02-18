#region Script Synopsis
    //Controls the tank turrent in the "TankPhysicsShooter" project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class TurretController : MonoBehaviour
    {
        public int Speed = 1;
        public Vector2 limit;
        private BasePattern controller;

        void Start()
        {
            controller = GetComponent<BasePattern>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
                controller.ParentRotation += Speed;
            else if (Input.GetKey(KeyCode.DownArrow))
                controller.ParentRotation -= Speed;

            if (controller.ParentRotation < limit.y)
                controller.ParentRotation = (int)limit.y;
            else if (controller.ParentRotation > limit.x)
                controller.ParentRotation = (int)limit.x;
        }
    }
}
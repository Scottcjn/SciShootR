#region Script Synopsis
    //Controls the pointer in the "PuzzleShooter" project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class PointerController : MonoBehaviour
    {
        public int Speed = 1;
        public int limit;

        private BasePattern controller;
        private BallReloader reLoader;
        private FireBullet point;

        void Start()
        {
            controller = GetComponent<BasePattern>();
            reLoader = GameObject.Find("BallReload").GetComponent<BallReloader>();
            point = GameObject.Find("Point").GetComponent<FireBullet>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow))
                controller.ParentRotation += Speed;
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
                controller.ParentRotation -= Speed;

            if (controller.ParentRotation < limit * -1)
                controller.ParentRotation = limit * -1;
            else if (controller.ParentRotation > limit)
                controller.ParentRotation = limit;

            if (!reLoader.isReady) return;

            if (Input.GetKeyDown(KeyCode.Space))
                controller.TriggerAutoFire = true;

            point.SpriteOverride = reLoader.BallAvailable;
        }
    }
}
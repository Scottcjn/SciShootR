#region Script Synopsis
    //A character controller used for inertial movement in the "OmniDirectionalShooter" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CharacterControllerInertial : MonoBehaviour
    {
        public int Speed = 1;
        public int Power = 100;
        private BasePattern controller;
        private Rigidbody2D rb;
        private bool blast;

        void Start()
        {
            controller = GetComponent<BasePattern>();
            rb = transform.parent.GetComponentInParent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (!blast) return;

            rb.AddForce(CalcObject.RotationToShotVector(transform.rotation.eulerAngles.z) * Power);
            blast = false;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow))
                controller.ParentRotation += Speed;
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow))
                controller.ParentRotation -= Speed;

            if (Input.GetKeyDown(KeyCode.Z))
                blast = true;
        }
    }
}
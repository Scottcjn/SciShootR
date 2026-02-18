#region Script Synopsis
    //A general purpose character controller used by playable and non-playable characters in many demo projects.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CharacterControllerMain : MonoBehaviour
    {
        [Range(1, 50)]
        public float Speed = 20;

        public bool auto;
        public Vector2 AutoRange;

        public bool UsesPhysics;

        public bool IgnoreXInput;
        public bool IgnoreYInput;

        public Transform Ground;

        private Vector2 startPos;
        private Rigidbody2D rb;

        private Vector3 inputDir;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;
        }

        void Update()
        {
            inputDir = input();

            if (UsesPhysics)
                return;

            if (auto)
                transform.position = lerp();
            else
                transform.position += inputDir * Time.deltaTime;
        }

        void FixedUpdate()
        {
            if (!UsesPhysics)
                return;

            if (!isGrounded())
                return;

            if (auto)
                rb.MovePosition(lerp());
            else
                rb.velocity = inputDir;
        }

        private Vector3 input()
        {
            Vector3 move = new Vector3();

            if (!IgnoreXInput)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    move.x = Speed * -1;
                else if (Input.GetKey(KeyCode.RightArrow))
                    move.x = Speed;
            }

            if (!IgnoreYInput)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                    move.y = Speed * -1;
                else if (Input.GetKey(KeyCode.UpArrow))
                    move.y = Speed;
            }

            return move;
        }

        private Vector2 lerp()
        {
            return new Vector2(
                Mathf.Lerp(startPos.x + AutoRange.x * -1, startPos.x + AutoRange.x, Mathf.PingPong(Time.time * Speed / 16.6f, 1)),
                Mathf.Lerp(startPos.y + AutoRange.y * -1, startPos.y + AutoRange.y, Mathf.PingPong(Time.time * Speed / 16.6f, 1))
            );
        }

        bool isGrounded()
        {
            if (Ground == null)
                return true;

            return Physics2D.OverlapPoint(Ground.position);
        }
    }
}
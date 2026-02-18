#region Script Synopsis
    //Demo script used for reloading balls in the "Puzzle Shooter" project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class BallReloader : MonoBehaviour
    {
        public Sprite[] Balls;
        public Sprite BallAvailable;
        public bool isReady;

        private float start = -42f;
        private float end = -34.6f;

        private float yPos;
        private float accumulator;

        private SpriteRenderer rend;

        void Start()
        {
            yPos = start;
            rend = GetComponent<SpriteRenderer>();
            BallAvailable = rend.sprite = getRandomBall();
        }

        void Update()
        {
            accumulator += Time.deltaTime;

            if (isReady)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    accumulator = 0;
                    isReady = false;
                    BallAvailable = rend.sprite = getRandomBall();
                }
            }
                
            yPos = Mathf.Lerp(start, end, accumulator);
            transform.position = new Vector2(transform.position.x, yPos);

            if (yPos == end)
                isReady = true;
        }
        
        private Sprite getRandomBall()
        {
            int randIndex = Random.Range(0, Balls.Length);
            return Balls[randIndex];
        }
    }
}
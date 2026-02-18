#region Script Synopsis
    //A general purpose script that dims a renderer's opacity. Used to flicker or dim backgrounds in several demo projects.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class FlickerBG : MonoBehaviour
    {
        public float Speed;
        public Vector2 Range;
        private SpriteRenderer bg;

        void Start()
        {
            bg = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            float opacity = Mathf.Lerp(Range.x, Range.y,
                Mathf.PingPong(Time.time * Speed, 1)
            );

            bg.color = new Color(1, 1, 1, opacity);
        }
    }
}
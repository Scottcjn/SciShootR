#region Script Synopsis
    //A simple script for scrolling the background. Used by "HorizontalShooter" and "VerticalShooter" to produce the illusion of motion.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class ScrollBG : MonoBehaviour
    {
        public bool YAxis;
        public float Speed = 1f;
        public float scroll;
        private float scrollLimit;

        void Start()
        {
            scrollLimit = scroll;
        }

        void Update()
        {  
            scroll -= Speed;
            scroll = (scroll <= scrollLimit * -1) ? scrollLimit : scroll;

            if (YAxis)
                transform.position = new Vector2(transform.position.x, scroll);
            else
                transform.position = new Vector2(scroll, transform.position.y);
        }
    }
}
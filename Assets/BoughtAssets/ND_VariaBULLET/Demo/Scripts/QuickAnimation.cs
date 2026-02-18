#region Script Synopsis
    //A Monobehavior variant of a BasicAnimation object except it attaches as a component to a gameobject to create animation.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class QuickAnimation : MonoBehaviour
    {
        public Sprite[] SpriteFrames;

        [Range(0, 60)]
        public int frameSkip;
        public bool FlickerSkipFrames;
        public int StartFrame = 0;
        protected BasicAnimation anim;
        protected SpriteRenderer rend;

        void Start()
        {
            rend = GetComponent<SpriteRenderer>();
            anim = new BasicAnimation(ref rend, ref SpriteFrames, StartFrame);
            anim.flickerSkipFrames = FlickerSkipFrames;
        }

        void Update()
        {
            anim.Animate(frameSkip);
        }
    }
}
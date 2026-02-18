#region Script Synopsis
    //An animatable bullet subclass. Allows for adding sprite frames in order to create a simple cycling anmiation.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShotBaseAnimatable : ShotBullet
    {
        [Header("Animation Settings")]

        [Tooltip("Sets optional animation frames for this shot.")]
        public Sprite[] Frames;

        [Tooltip("Sets interval for changing frames. [Lower number = faster animation].")]
        public int FrameSkip;

        [Tooltip("Sets initial animation frame randomly.")]
        public bool RandomStartFrame;

        private BasicAnimation anim;

        public override void InitialSet()
        {
            base.InitialSet();

            if (RandomStartFrame)
            {
                int randIndex = (int)Random.Range(0, Frames.Length - 1);
                anim = new BasicAnimation(ref rend, ref Frames, randIndex);
            }
            else
                anim = new BasicAnimation(ref rend, ref Frames);
        }

        public override void Update()
        {
            base.Update();
            anim.Animate(FrameSkip);
        }

        protected override void setSprite(SpriteRenderer sr)
        {
            if (Frames.Length == 0)
                base.setSprite(sr);
            else
            {
                sr.color = FiringScript.SpriteColor;
                sr.sortingLayerName = sortLayer;
                sr.sortingOrder = sortOrder;
            }
        }
    }
}
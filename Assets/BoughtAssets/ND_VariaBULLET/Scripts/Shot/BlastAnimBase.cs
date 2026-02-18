#region Script Synopsis
    //Base class for creating a "blast" animation. Used for ships/missiles which create a rear blast upon a trigger state occurring. 
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class BlastAnimBase : MonoBehaviour
    {
        public Sprite[] Frames;
        private BasicAnimation anim;

        protected bool trigger;

        private int frameSkip = 2;
        private int blastCounter;
        public bool blastOnYAxis;
        public float blastNeutral;
        public float blastOffset;

        protected virtual void Start()
        {
            var rend = GetComponent<SpriteRenderer>();
            anim = new BasicAnimation(ref rend, ref Frames);
        }

        protected virtual void Update()
        {
            if (trigger)
            {
                blastCounter = ((frameSkip * Frames.Length) + (Frames.Length + 1)) * 2;
                anim.Reset(0);
            }
                
            blastCounter = (blastCounter > 0) ? blastCounter - 1 : 0;

            if (blastCounter > 0)
                transform.localPosition = new Vector2((!blastOnYAxis) ? blastOffset : 0, (blastOnYAxis) ? blastOffset : 0);
            else
                transform.localPosition = new Vector2((!blastOnYAxis) ? blastNeutral : 0, (blastOnYAxis) ? blastNeutral : 0);

            anim.Animate(frameSkip);
        }
    }
}
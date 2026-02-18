#region Script Synopsis
    //A general purpose script that moves between color values. Used in several demo projects for added visual flare.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class ColorLerp : MonoBehaviour
    {
        public Color From;
        public Color To;

        [Range(1, 10)]
        public float Speed;

        public bool OnCommand;
        public KeyCode CommandKey;

        private float accumulator;
        private SpriteRenderer rend;

        private Timer timer;
        private bool trigger;

        void Start()
        {
            if (OnCommand)
                timer = new Timer(0);

            rend = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (OnCommand)
                CommandedColorChange();
            else
                ConstantColorChange();
        }

        private void CommandedColorChange()
        {
            if (trigger)
                timer.RunOnce(Speed);
            else if (Input.GetKey(CommandKey))
                { trigger = true; rend.color = To; }

            if (timer.Flag)
            {
                if (Input.GetKey(CommandKey))
                    return;

                timer.Reset();
                trigger = false;
                rend.color = From;
            }
        }

        private void ConstantColorChange()
        {
            accumulator += Time.deltaTime;
            rend.color = Color.Lerp(From, To, Mathf.PingPong(accumulator * Speed, 1));
        }
    }
}
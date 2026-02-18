#region Script Synopsis
    //An animation object which manages the sprites, renderer, and frame-skip of the object which contains it.
    //Example: ShotBaseAnimatable.anim
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class BasicAnimation
    {
        private int previousSkip;

        private Sprite[] frames;
        private SpriteRenderer rend;

        private int _frameNum;
        public int FrameNum
        {
            get { return _frameNum; }
            set
            {
                _frameNum = (_frameNum == frames.Length - 1) ? 0 : value;
                rend.sprite = frames[_frameNum];
            }
        }

        public bool flickerSkipFrames = false;
        private bool flickerToggle;
        private bool stopNextFrame;
        private Timer timer = new Timer(0);

        public BasicAnimation(ref SpriteRenderer rend, ref Sprite[] frames)
        {
            this.rend = rend;
            this.frames = frames;
           
            if (rend != null && frames.Length > 0) //takes the initial frame and sets it to the renderer
                Reset(0);
        }

        public BasicAnimation(ref SpriteRenderer rend, ref Sprite[] frames, int startFrame)
        {
            this.rend = rend;
            this.frames = frames;

            if (rend != null && frames.Length > 0)
                Reset(startFrame);
        }

        public bool AnimateOnce(int skip)
        {
            if (stopNextFrame && timer.Flag)
                return true;

            timer.Run(skip);

            if (timer.Flag)
            {
                FrameNum++;

                if (FrameNum == frames.Length - 1)
                    stopNextFrame = true;

                flickerToggle = true;
            }
            else
                if (flickerSkipFrames)
                    flickerToggle = !flickerToggle;


            rend.enabled = flickerToggle;
            return false;
        }

        public void Animate(int skip)
        {
            if (frames.Length < 1)
                return;
            else if (frames.Length == 1) { SyncAnimate(0); return; }

            if (skip != previousSkip)
                timer.Reset();

            timer.Run(skip);

            if (timer.Flag) { FrameNum++; flickerToggle = true; }
            else
                if (flickerSkipFrames)
                    flickerToggle = !flickerToggle;

            rend.enabled = flickerToggle;
            previousSkip = skip;
        }

        public void SyncAnimate(int syncFrame)
        {
            if (previousSkip == syncFrame) {
                if (flickerSkipFrames)
                    flickerToggle = !flickerToggle;
            }
            else
                flickerToggle = true;

            rend.sprite = frames[syncFrame];
            rend.enabled = flickerToggle;
            previousSkip = syncFrame;
        }

        public void Reset(int startFrame)
        {
            FrameNum = startFrame;
            timer.Reset();
            flickerToggle = true;
            stopNextFrame = false;
        }
    }
}
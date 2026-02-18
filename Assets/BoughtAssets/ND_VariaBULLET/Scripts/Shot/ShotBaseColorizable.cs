#region Script Synopsis
    //A bullet subclass. Allows for adding color and opacity animations.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class ShotBaseColorizable : ShotBaseRotatable
    {
        private Timer DissipateTimer = new Timer(0);

        [Header("Colorize Settings")]

        [Range(0, 100)]
        [Tooltip("Sets delay in frames before onset of opacity reduction (dissipate).")]
        public int DissipateDelay;

        [Range(0, 200)]
        [Tooltip("Sets the speed at which opacity reduces (dissipates).")]
        public int DissipateSpeed;
        private float dissipateAccumulator;

        [Tooltip("Sets color indexes to loop through which creates a color shifting effect.")]
        public Color[] ColorShift;
        private int shiftIndex;
        private int shiftDir = 1;

        [Range(0, 100)]
        [Tooltip("Sets the cycling speed for colors set in ColorShift.")]
        public int colorShiftSpeed;

        [Tooltip("Starts with a random start color index set in ColorShift.")]
        public bool randomStartColor;
        private float shiftAccumulator;

        [Range(0.1f, 1f)]
        [Tooltip("Sets the starting opacity for this shot.")]
        public float Opacity = 1;

        private bool staticColor;
        private bool staticOpacity;

        public override void InitialSet()
        {
            base.InitialSet();

            if (ColorShift.Length >= 2)
            {
                rend.color = ColorShift[0];
                shiftAccumulator = 0;
                shiftIndex = (randomStartColor) ? (int)Random.Range(0, ColorShift.Length - 1) : 0;
            }
            else
                staticColor = true;

            if (DissipateSpeed > 0)
            {
                DissipateTimer.Reset();
                dissipateAccumulator = 0;
            }
            else
                staticOpacity = true;

            rend.color = new Color(rend.color.r,rend.color.g, rend.color.b, Opacity); //to ensure opacity is set on 1st frame
        }

        public override void Update()
        {
            base.Update();

            Color newColor = colorLerp();
            newColor.a = opacityLerp();
            rend.color = newColor;
        }

        private Color colorLerp()
        {
            float r;
            float g;
            float b;

            if (!staticColor)
            {
                if (shiftIndex == 0)
                    shiftDir = 1;
                else if (shiftIndex == ColorShift.Length - 1)
                    shiftDir = -1;

                shiftAccumulator += Time.deltaTime / 8;
                r = Mathf.Lerp(ColorShift[shiftIndex].r, ColorShift[shiftIndex + shiftDir].r, shiftAccumulator * colorShiftSpeed);
                g = Mathf.Lerp(ColorShift[shiftIndex].g, ColorShift[shiftIndex + shiftDir].g, shiftAccumulator * colorShiftSpeed);
                b = Mathf.Lerp(ColorShift[shiftIndex].b, ColorShift[shiftIndex + shiftDir].b, shiftAccumulator * colorShiftSpeed);

                if (shiftAccumulator * colorShiftSpeed >= 1)
                {
                    shiftAccumulator = 0;
                    shiftIndex += shiftDir;
                }
            }
            else
            {
                r = rend.color.r;
                g = rend.color.g;
                b = rend.color.b;
            }

            return new Color(r, g, b);
        }

        private float opacityLerp()
        {
            if (staticOpacity)
                return Opacity;

            DissipateTimer.RunOnce(DissipateDelay);

            float a = Opacity;
            if (DissipateTimer.Flag)
            {
                dissipateAccumulator += Time.deltaTime / 8;
                a = Mathf.Lerp(Opacity, 0, dissipateAccumulator * DissipateSpeed);

                if (a <= 0.01f)
                    RePoolOrDestroy();
            }
            return a;
        }
    }
}
#region Script Synopsis
//Linear automator, which automates controller parameters in a fluid progression.
//Becomes attached to a controller via the attached spreadpattern (controller) script.
//Learn more about automators at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#automators
#endregion

using System;
using UnityEngine;

namespace ND_VariaBULLET
{
    public class AutomateLinear : AutomateBase
    {
        [Tooltip("Sets the initial value for the control being modified.")]
        public float From;

        [Tooltip("Sets the end value for the control being modified.")]
        public float To;

        [Tooltip("Sets the speed at which values gradually modify between From and To.")]
        public float Speed;

        [Tooltip("Creates a smoothing effect between the from/to values.")]
        public bool Ease;

        [Tooltip("Define a custom rate of change, represented as a curvature, between the from/to values when Behavior is set to 'Single Pass' or 'Ping Pong' [Undefined Curve = Linear].")]
        public AnimationCurve Curve;

        private Func<float, float, float, float>[] lerpMethod;

        protected override void Awake()
        {
            base.Awake();
            lerpMethod = new Func<float, float, float, float>[2] { Mathf.Lerp, Mathf.SmoothStep };
            lerpCurveInit();
        }

        void Update()
        {
            delay.RunOnce(InitialDelay);
            if (!delay.Flag) return;

            accumulator += Time.deltaTime;
            controlLink[Destination]((method(From, To, Speed)));
        }

        protected override float SinglePass(float from, float to, float speed)
        {
            float difference = from - to;
            float relativeSpeed = (difference != 0) ? speed / Mathf.Abs(difference) : speed;

            return lerpMethod[Convert.ToInt16(Ease)](from, to, Curve.Evaluate(accumulator * relativeSpeed));
        }

        protected override float Continuous(float from, float to, float speed)
        {
            float direction = (from <= to) ? 1 : -1;
            return accumulator * speed * direction;
        }

        protected override float PingPong(float from, float to, float speed)
        {

            float difference = from - to;
            float relativeSpeed = (difference != 0) ? speed / Mathf.Abs(difference) : speed;
            return lerpMethod[Convert.ToInt16(Ease)](from, to, Curve.Evaluate(Mathf.PingPong(accumulator * relativeSpeed, 1)));
        }

        protected override float Randomized(float from, float to, float speed)
        {
            return UnityEngine.Random.Range(from, to);
        }

        private void lerpCurveInit()
        {
            if (Curve.keys.Length == 0)
            {
                Curve.AddKey(0, 0);
                Curve.AddKey(1, 1);
            }
        }
    }
}
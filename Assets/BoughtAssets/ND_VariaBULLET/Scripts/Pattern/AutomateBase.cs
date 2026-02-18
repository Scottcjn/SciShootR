#region Script Synopsis
    //Base class for AutomateLinear and AutomateStepped, which become attached to controller gameobjects via the SpreadPattern (controller) script.
    //Learn more about automators at: https://neondagger.com/variabullet2d-in-depth-controller-guide/#automators
#endregion

using UnityEngine;
using System.Collections.Generic;
using System;

namespace ND_VariaBULLET
{
    [RequireComponent(typeof(BasePattern))]
    public abstract class AutomateBase : MonoBehaviour
    {
        [Tooltip("Sets which control point this automator modifies in the controller.")]
        public ControlType Destination;

        [Tooltip("Sets the behavior of the control modification.")]
        public ControlBehavior Behavior;

        [Tooltip("Sets a delay in frames before control modification starts.")]
        public int InitialDelay = 0;
        protected Timer delay;

        protected Dictionary<ControlType, Action<float>> controlLink;
        protected Func<float, float, float, float> method;

        protected SpreadPattern pattern;
        protected float accumulator;

        protected virtual void Awake()
        {
            delay = new Timer(0);
            pattern = GetComponent<SpreadPattern>();

            controlLink = new Dictionary<ControlType, Action<float>>() {
                { ControlType.ParentRotation, parentRotation },
                { ControlType.CenterRotation, centerRotation },
                { ControlType.ExitPointOffset, exitPointOffset },
                { ControlType.Pitch, pitch },
                { ControlType.SpreadDegrees, spreadDegrees },
                { ControlType.SpreadRadius, spreadRadius },
                { ControlType.SpreadX, spreadX },
                { ControlType.SpreadY, spreadY }
            };

            var behaviors = new Func<float, float, float, float>[4] {
                SinglePass, Continuous, PingPong, Randomized
            };

            method = behaviors[(int)Behavior];
        }

        protected abstract float SinglePass(float p1, float p2, float p3);
        protected abstract float Continuous(float p1, float p2, float p3);
        protected abstract float PingPong(float p1, float p2, float p3);
        protected abstract float Randomized(float p1, float p2, float p3);

        protected void parentRotation(float val) { pattern.ParentRotation = val; }
        protected void centerRotation(float val) { pattern.CenterRotation = val; }
        protected void exitPointOffset(float val) { pattern.ExitPointOffset = val; }
        protected void pitch(float val) { pattern.Pitch = val; }
        protected void spreadDegrees(float val) { pattern.SpreadDegrees = val; }
        protected void spreadRadius(float val) { pattern.SpreadRadius = val; }
        protected void spreadX(float val) { pattern.SpreadXAxis = val; }
        protected void spreadY(float val) { pattern.SpreadYAxis = val; }

        public enum ControlType
        {
            ExitPointOffset,
            Pitch,
            SpreadDegrees,
            SpreadRadius,
            CenterRotation,
            SpreadY,
            SpreadX,
            ParentRotation
        }

        public enum ControlBehavior
        {
            SinglePass = 0,
            Continuous,
            PingPong,
            Randomized
        }
    }
}
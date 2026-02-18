#region Script Synopsis
    //An object that stores a typical counter as a float and increments/resets via available methods.
    //Examples: most places a counter is used (and particularly if it needs to be framerate independent)
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET
{
    public struct Timer
    {
        public bool Flag;
        public static float deltaCounter { get { return Time.deltaTime * 60; } }
        private float counter;
        public float GetCounter { get { return counter; } }
        private float reset;

        public Timer(float startsAt)
        {
            Flag = false;
            counter = reset = startsAt;
        }

        public void Run(float limit)
        {
            if (counter < limit)
            {
                counter += deltaCounter;
                Flag = false;
            }
            else
            {
                counter = reset;
                Flag = true;
            }
        }

        public IEnumerator RunForFrames(float limit, System.Action<float> action)
        {
            while (counter < limit)
            {
                action(counter);

                counter += deltaCounter;
                Flag = false;

                yield return null;
            }

            counter = reset;
            Flag = true;
        }

        public IEnumerator WaitForFrames(float limit)
        {
            while (counter < limit)
            {
                counter += deltaCounter;
                yield return null;
            }

            counter = reset;
        }

        public void RunOnce(float limit)
        {
            if (Flag)
                return;

            counter += deltaCounter;
            Flag = !(counter < limit);
        }

        public void Reset()
        {
            counter = reset;
            Flag = false;
        }

        public void ForceFlag(float limit)
        {
            counter = limit;
        }
    }
}
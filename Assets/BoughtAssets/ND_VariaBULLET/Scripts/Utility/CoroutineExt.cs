#region Script Synopsis
    //A set of common utility methods that can be called as/within coroutines.
#endregion

using UnityEngine;
using System;
using System.Collections;

namespace ND_VariaBULLET
{
    public static class CoroutineExt
    {
        public static IEnumerator DoForFrames(int frames, Action<float> action)
        {
            yield return new Timer(0).RunForFrames(frames, action);
        }

        public static IEnumerator DoOnce(Action action)
        {
            action();
            yield return null;
        }

        public static IEnumerator WaitForFramesDo(int frames, Action action)
        {
            yield return new Timer(0).WaitForFrames(frames);
            action();
        }

        public static IEnumerator WaitFoFrames(int frames)
        {
            yield return new Timer(0).WaitForFrames(frames);
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }
}
#region Script Synopsis
    //Utility methods used by homing shots types for common calculations.
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET
{
    internal class HomingCalc
    {
        internal Timer recalcTimer = new Timer(0);

        internal void recalcClosestObject(Transform srcObject, ref Transform objectToFollow, int fps, string targetFromTag)
        {
            recalcTimer.Run(fps);
         
            if (recalcTimer.Flag) //run recalc only every n frames, less costly and more realistic behavior
            {
                if (!String.IsNullOrEmpty(targetFromTag))
                    objectToFollow = findClosestObject(srcObject, targetFromTag);
            }
        }

        internal Transform findClosestObject(Transform srcObject, string targetFromTag)
        {
            GameObject closest = null;
            float distance = Mathf.Infinity;

            foreach (GameObject gO in GameObject.FindGameObjectsWithTag(targetFromTag))
            {
                float currentResult = Vector3.Distance(srcObject.position, gO.transform.position);

                if (currentResult < distance) { distance = currentResult; closest = gO; }
            }

            if (closest != null)
                return closest.transform;
            else
                return null;
        }

        internal bool isWithinRadius(Transform srcObject, Transform objectToFollow, float engageRadius)
        {
            if (objectToFollow != null)
            {
                float x = Mathf.Abs(srcObject.position.x - objectToFollow.position.x);
                float y = Mathf.Abs(srcObject.position.y - objectToFollow.position.y);

                if (x <= engageRadius && y <= engageRadius)
                    return true;
            }

            return false;
        }
    }
}
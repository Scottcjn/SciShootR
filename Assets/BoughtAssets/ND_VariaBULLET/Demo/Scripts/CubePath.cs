#region Script Synopsis
    //A single-purpose script used in "LaserSaws" demo project in order to move the cube object along a pre-determined path.
#endregion

using UnityEngine;
using System;

namespace ND_VariaBULLET.Demo
{
    public class CubePath : MonoBehaviour
    {
        private float accumulator;
        private int currentStep;
        private Action[] path;

        void Start()
        {
            path = new Action[]
            {
                delegate() { transform.position = new Vector2(Mathf.Lerp(25.75f, -25.75f, accumulator), transform.position.y); },
                delegate() { transform.position = new Vector2(transform.position.x, Mathf.Lerp(7.5f, -6.1f, accumulator)); },
                delegate() { transform.position = new Vector2(Mathf.Lerp(-25.75f, 25.75f, accumulator), transform.position.y); },
                delegate() { transform.position = new Vector2(transform.position.x, Mathf.Lerp(-6.1f, 7.5f, accumulator)); }
            };
        }

        void Update()
        {
            accumulator += Time.deltaTime / 2;

            if (accumulator >= 1)
            {
                accumulator = 0;
                currentStep++;

                if (currentStep > path.Length - 1)
                    currentStep = 0;
            }

            path[currentStep]();
        }
    }
}
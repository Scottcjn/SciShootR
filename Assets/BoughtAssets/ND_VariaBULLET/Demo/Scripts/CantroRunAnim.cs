#region Script Synopsis
    //Controls the running sprite animation used in the "RunNGun" demo project.
#endregion

using UnityEngine;
using System.Collections;

namespace ND_VariaBULLET.Demo
{
    public class CantroRunAnim : QuickAnimation
    {
        void Update()
        {
            animate();
            direction();
        }

        private void direction()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                transform.localScale = new Vector3(-1, 1, 1);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                transform.localScale = new Vector3(1, 1, 1);
        }

        private void animate()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                anim.Reset(0);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                anim.Animate(frameSkip);
            else
                rend.sprite = SpriteFrames[2];
        }
    }
}
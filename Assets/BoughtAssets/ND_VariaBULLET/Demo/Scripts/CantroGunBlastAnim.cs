#region Script Synopsis
    //Controls the "blast" animation used by the weapon in the "RunNGun" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class CantroGunBlastAnim : QuickAnimation
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                anim.Reset(0);

            if (Input.GetKey(KeyCode.Space))
                anim.Animate(frameSkip);
            else
                rend.sprite = null;
        }
    }
}
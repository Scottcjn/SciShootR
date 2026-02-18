#region Script Synopsis
    //Variant of BlastAnimBase used in the "OmniDirectionalShooter" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class BlastShip : BlastAnimBase
    {
        protected override void Update()
        {
            trigger = Input.GetKeyDown(KeyCode.Z);
            base.Update();
        }
    }
}
#region Script Synopsis
//A simple utility script for forcing GlobalShotManager instantiation at runtime, for performance.
//DemoVersion is enabled to load the default GlobalShotManager used for demo projects.
#endregion

using UnityEngine;
using System.IO;

namespace ND_VariaBULLET
{
    public class ForceGlobalShotManager : MonoBehaviour
    {
        [Tooltip("Loads the demo version of the GlobalShotManager. Should ONLY be used for VariaBULLET2D Demo scenes.")]
        public bool DemoVersion;

        [Tooltip("Suppresses warnings on play related to TagManager/Physics2D settings and GlobalShotMangager version.")]
        public bool SuppressWarning;

        void Awake() //required in case another script references GlobalShotManager.instance before this does
        {
            string version = "";
            if (DemoVersion)
            {
                GlobalShotManager.DemoMode = true;
                version = " (Demo Version)";

                if (!SuppressWarning && (!File.Exists("Assets/ND_VariaBULLET/System/UserBackup/TagManager.asset_backup") || !File.Exists("Assets/ND_VariaBULLET/System/UserBackup/Physics2DSettings.asset_backup")))
                    Utilities.Warn("TagManager/Physics2D settings have never been established via the Menu > VariaManager > Replace procedures | " + "Demo projects and general operation may not work as expected. For more info, see section 6.1 Collision System Automatic Setup in the System Guide documentation.");
            }

            GlobalShotManager g = GlobalShotManager.Instance;

            if (!SuppressWarning)
                Utilities.Warn(g.name + version + " instantiated at Start by " + this.ToString() + ". This is a notice to ensure the correct GlobalShotManager is being used");
        }
    }
}
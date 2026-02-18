#region Script Synopsis
    //A special-purpose custom preset switcher used by the "OmniDirectionalShooter" demo project.
#endregion

using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class OmniShooterSwitcher : MonoBehaviour
    {
        public KeyCode buttonSwitch;
        public GameObject[] presetPrefabs;

        private int _index;
        private int index
        {
            get { return (_index == presetPrefabs.Length) ? 0 : _index; }
            set { _index = value; }
        }

        private FireBase firingScript;

        void Start()
        {
            firingScript = GetComponent<FireBase>();
        }

        void Update()
        {
            if (Input.GetKeyDown(buttonSwitch))
            {
                index++;
                firingScript.Shot = presetPrefabs[index];
            }
                
        }
    }
}
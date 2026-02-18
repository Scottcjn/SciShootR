#region Script Synopsis
    //A script attached to any gameobject which is parent to an Origin (controller). Can change to a different pattern on-the-fly when triggered.
    //Learn more about preset switching at: https://neondagger.com/variabullet2d-system-guide/#preset-switcher
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class FullPresetSwitcher : MonoBehaviour
    {
        [Tooltip("Button for switching between presets, best used for testing.")]
        public KeyCode buttonSwitch;

        [Tooltip("Trigger for switching between presets, set elsewhere in code.")]
        public bool triggerSwitch;

        [Tooltip("Populate presets to switch through in sequential order.")]
        public GameObject[] presetPrefabs;
        protected int index;

        [Tooltip("Sets delay in frames before preset is active after switching to it.")]
        public int DelayFrames = 20;
        private Timer delayTimer;
        private GameObject newPreset;

        [Tooltip("Automatically loads the first preset in presetPrefabs on Start.")]
        public bool AutoSwitchOnStart;

        void Start()
        {
            if (AutoSwitchOnStart) { destroyCurrent(); applyPreset(presetPrefabs[0], true); }
            
            delayTimer = new Timer(0);
            delayTimer.ForceFlag(DelayFrames);
        }

        void Update()
        {
            if (isPresetChangeTriggered())
                delayTimer.Reset();

            activatePresetAfter(DelayFrames);
        }

        private bool isPresetChangeTriggered()
        {
            if ((!Input.GetKeyDown(buttonSwitch) && !triggerSwitch) || !delayTimer.Flag) return false;

            destroyCurrent();

            index++;
            if (index > presetPrefabs.Length - 1)
                index = 0;

            applyPreset(presetPrefabs[index], false);
            triggerSwitch = false;

            return true;
        }

        private void applyPreset(GameObject selection, bool isActive)
        {
            newPreset = Instantiate(selection);
            Vector2 storedPosition = newPreset.transform.localPosition;

            newPreset.transform.parent = this.transform;
            newPreset.transform.localPosition = storedPosition;
            newPreset.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);   
            newPreset.transform.localScale = new Vector3(1, 1, 1);
            newPreset.SetActive(isActive);
        }

        private void activatePresetAfter(int delay)
        {
            delayTimer.RunOnce(delay);

            if (newPreset == null)
                return;

            if (delayTimer.Flag)
                { newPreset.SetActive(true); newPreset = null; }
        }

        private void destroyCurrent()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Origin" || child.gameObject.name == "Origin(Clone)")
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }
}
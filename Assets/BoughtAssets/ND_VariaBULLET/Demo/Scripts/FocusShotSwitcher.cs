using UnityEngine;

namespace ND_VariaBULLET.Demo
{
    public class FocusShotSwitcher : MonoBehaviour
    {
        private CharacterControllerMain charControl;
        private FullPresetSwitcher switcher;
        private KeyCode switchButton;
        private float originalSpeed;

        private bool toggle;

        void Start()
        {
            charControl = GetComponent<CharacterControllerMain>();
            switcher = GetComponent<FullPresetSwitcher>();
            switchButton = switcher.buttonSwitch;

            originalSpeed = charControl.Speed;
        }

        void Update()
        {
            if (Input.GetKeyDown(switchButton))
                toggle = !toggle;

            charControl.Speed = toggle ? (int)(originalSpeed * .7f) : originalSpeed;
        }
    }
}

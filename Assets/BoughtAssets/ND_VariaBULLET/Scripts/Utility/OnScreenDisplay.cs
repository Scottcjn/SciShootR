#region Script Synopsis
    //An On-Screen Display attached to the GlobalShotManager prefab. Used for displaying various diagnostics.
#endregion

using UnityEngine;

namespace ND_VariaBULLET
{
    public class OnScreenDisplay : MonoBehaviour
    {
        public GUIStyle FontStyle = new GUIStyle();
        private int scaledFont { get { return (Screen.width + Screen.height) / 140; } }

        void OnGUI()
        {
            FontStyle.fontSize = scaledFont;
            drawOSD();
        }

        void drawOSD()
        {
            GUI.Label(
                new Rect(Screen.width / 85f, Screen.height / 48, Screen.width, Screen.height),
                string.Format("SpeedScale: {0}", GlobalShotManager.Instance.SpeedScale.ToString()), FontStyle
            );
            GUI.Label(
                new Rect(Screen.width / 85f, Screen.height / 24, Screen.width, Screen.height),
                string.Format("Active Bullets: {0}", GlobalShotManager.Instance.ActiveBullets.ToString()), FontStyle
            );
            GUI.Label(
                new Rect(Screen.width / 85f, Screen.height / 16, Screen.width, Screen.height),
                string.Format("Target FPS: {0}", GlobalShotManager.Instance.TestFrameRate.ToString()), FontStyle
            );
            GUI.Label(
                new Rect(Screen.width / 85f, Screen.height / 12, Screen.width, Screen.height),
                string.Format("Actual FPS: {0}", (1f / Time.deltaTime).ToString()), FontStyle
            );
            GUI.Label(
                new Rect(Screen.width / 85f, Screen.height / 9.6f, Screen.width, Screen.height),
                string.Format("Throttling: {0}", (GlobalShotManager.Instance.EmulateCPUThrottle) ? (Time.timeScale * 100).ToString() + "%" : "Disabled"), FontStyle
            );

        }
    }
}
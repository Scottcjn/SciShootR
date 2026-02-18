#region Script Synopsis
    //For adding buttons to a controller's SpreadPattern in the inspector, calling its methods for adding linear/stepped automators.
#endregion

using UnityEngine;
using UnityEditor;

namespace ND_VariaBULLET.EditorGUI
{
    [CustomEditor(typeof(BasePattern), true)]
    public class BasePatternEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Color cachedBG = GUI.backgroundColor;

            DrawDefaultInspector();
            var bsScript = (SpreadPattern)target;

            GUI.backgroundColor = new Color(.98f, .98f, 0);
            if (GUILayout.Button("Clear Cache"))
                bsScript.clearEmitterCache();

            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Clone First Emitter To All"))
                bsScript.cloneFirstEmitter();

            GUI.backgroundColor = new Color(.46f, .64f, 0);
            if (GUILayout.Button("Add Linear Automator"))
                bsScript.addAutomator("Linear");

            if (GUILayout.Button("Add Stepped Automator"))
                bsScript.addAutomator("Stepped");

            GUI.backgroundColor = cachedBG;          
        }
    }
}

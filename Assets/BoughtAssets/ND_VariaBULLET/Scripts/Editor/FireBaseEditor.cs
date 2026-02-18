#region Script Synopsis
    //For adding a buttons to a firing script, calling its method that adds sets up an audio source.
#endregion

using UnityEngine;
using UnityEditor;

namespace ND_VariaBULLET.EditorGUI
{
    [CustomEditor(typeof(FireBase), true)]
    public class FireBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Color cachedBG = GUI.backgroundColor;

            DrawDefaultInspector();
            var fbScript = (FireBase)target;

            GUI.backgroundColor = new Color(1, 0.70f, 0);
            if (GUILayout.Button("Connect Audio Event"))
                fbScript.ConnectAudio();

            GUI.backgroundColor = cachedBG;
        }
    }
}
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class RatePopUp : EditorWindow
{
	//Stores registry entries under these names. In WIN, found in HKCU/Software/Unity Technologies/Unity Editor 5.x/
	const string savedKillPref = "variaBULLETKillPopUp";
    const string installDate = "variaBULLETInstallDate";
    
    const string assetStoreLink = "https://assetstore.unity.com/packages/templates/systems/variabullet2d-projectile-system-152528";
    const int minDaysTilNag = 7;
    bool isPopUpKilled = false;
	
    static RatePopUp()
    {
		double secsSinceStart = EditorApplication.timeSinceStartup;

        if (secsSinceStart < 60)
			EditorApplication.update += runOnLoad;
    }

    private static void runOnLoad()
    {
        EditorApplication.update -= runOnLoad;

		if (Application.isPlaying)
            return;
		
		string installedOn = EditorPrefs.GetString(installDate);
		
		if (installedOn == "")
			EditorPrefs.SetString(installDate, DateTime.Now.ToString());
		else if (!EditorPrefs.GetBool(savedKillPref))
			if ((DateTime.Now - DateTime.Parse(installedOn)).TotalDays > minDaysTilNag)
				initWindow();
    }

    private static void initWindow()
    {
        RatePopUp window = EditorWindow.GetWindow<RatePopUp>(true, "Thank you!", true);
		window.maxSize = new Vector2(360, 300);
		window.minSize = window.maxSize;
    }

    private void OnGUI()
    {
        //TITLE
        GUILayout.Label(
            "Thank you for purchasing VariaBULLET2D!",
            new GUIStyle(EditorStyles.largeLabel) { wordWrap = true, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperLeft, richText = true }
        );

        //BODY
        GUILayout.Label(
            "NeonDagger is a small development studio with big goals. But we can't do it on our own :(\n\n" + "Rating the asset package helps us immeasurably towards developing and supporting new content and keeping this gamedev dream alive! \n\n" +
            "If you have any questions or concerns, please check out our extensive documentation and reach out to us at neondagger.com  \n",
            new GUIStyle(EditorStyles.label) { wordWrap = true, alignment = TextAnchor.UpperLeft, richText = true }
        );

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();

        Color col = GUI.color;
        GUI.backgroundColor = Color.magenta;

        //BUTTON
        if (GUILayout.Button("Rate VariaBULLET2D", GUILayout.MinHeight(30), GUILayout.MaxWidth(300)))
            Application.OpenURL(assetStoreLink);

        GUI.backgroundColor = col;

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(30);

        //KILL-TOGGLE
        isPopUpKilled = GUILayout.Toggle(isPopUpKilled, "Do not show this again.");
    }

    public void OnDestroy()
    {
        EditorPrefs.SetBool(savedKillPref, isPopUpKilled);
    }
}

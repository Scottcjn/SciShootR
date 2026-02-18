#region Script Synopsis
    //A procedure that backs up existing TagManager/Physics2D settings, and overwriting with the settings used in the demo projects or fresh projects.
    //Places a menu item in the editor for execution.
    //Usage is covered in depth at http://neondagger.com/variabullet2d-system-guide/#automatic-setup
#endregion

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace ND_VariaBULLET.EditorGUI
{
    public static class VariaManager
    {
        private const string backupDir = "Assets/ND_VariaBULLET/System/UserBackup/";
        private const string replaceDir = "Assets/ND_VariaBULLET/System/VariaReplacement/";
        private const string liveDir = "ProjectSettings/";

        static string TagManagerBackUp = backupDir + "TagManager.asset_backup";
        static string TagManagerReplacement = replaceDir + "TagManager.asset_replacement";
        static string TagManagerLive = liveDir + "TagManager.asset";

        static string Physics2DBackUp = backupDir + "Physics2DSettings.asset_backup";
        static string Physics2DReplacement = replaceDir + "Physics2DSettings.asset_replacement";
        static string Physics2DLive = liveDir + "Physics2DSettings.asset";

        // MENU RESTORE //
        [MenuItem("VariaManager/Restore Original TagManager Settings")]
        static void MenuRestoreOriginalTagManager()
        {
            restoreOriginal(TagManagerBackUp, TagManagerLive);
        }

        [MenuItem("VariaManager/Restore Original Physics2D Settings")]
        static void MenuRestoreOriginalPhysics2D()
        {
            restoreOriginal(Physics2DBackUp, Physics2DLive);
        }

        // MENU REPLACE //
        [MenuItem("VariaManager/Replace TagManager Settings")]
        static void MenuReplaceWithVariaTagManager()
        {
            replaceWithVaria(TagManagerLive, TagManagerBackUp, TagManagerReplacement, TagManagerLive);
        }

        [MenuItem("VariaManager/Replace Physics2D Settings")]
        static void MenuReplaceWithVariaPhysics2D()
        {
            replaceWithVaria(Physics2DLive, Physics2DBackUp, Physics2DReplacement, Physics2DLive);
        }


        //UTILITY METHODS //

        static bool backupOriginal(string src, string dest)
        {
            if (File.Exists(dest))
            {
                Debug.Log("Backup already exists!");
                return true;
            }
            else
            {
                try
                {
                    FileUtil.CopyFileOrDirectory(src, dest);
                    success("backing up", src, dest);
                    return true;
                }
                catch (Exception e)
                {
                    error("backing up", e);
                    return false;
                }
            }
        }

        static void restoreOriginal(string src, string dest)
        {
            if (!File.Exists(src))
                Debug.Log("Backup does not exist! You will have to run the backup first to create the file (Backup occurs automatically when the replacement procedure is run).");
            else
            {
                if (EditorUtility.DisplayDialog("Confirmation", "This procedure restores your original files. Do you you wish to continue?", "YES", "NO"))
                {
                    try
                    {
                        FileUtil.ReplaceFile(src, dest);
                        success("restoring", src, dest);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception e)
                    {
                        error("restoring", e);
                    }
                }
                else
                    Debug.Log("Procedure Canceled");
            }
        }

        static void replaceWithVaria(string backupSrc, string backupDest, string src, string dest)
        {
            if (!File.Exists(backupDest))
            {
                Debug.Log("Backup does not exist. Creating backup...");

                if (!backupOriginal(backupSrc, backupDest))
                {
                    Debug.Log("Replacement procedure cannot continue.");
                    return;
                }
                else
                    Debug.Log("Ready for replacement procedure...");
            }


            if (!File.Exists(src))
                Debug.Log("Replacement does not exist. Cannot run automated replacement routine.");
            else
            {
                if (EditorUtility.DisplayDialog("Confirmation", "This procedure replaces your original files with VariaBULLET2D's versions. Do you you wish to continue?", "YES", "NO"))
                {
                    try
                    {
                        FileUtil.ReplaceFile(src, dest);
                        success("replacing", src, dest);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception e)
                    {
                        error("replacing", e);
                    }
                }
                else
                    Debug.Log("Procedure Canceled");
            }
        }

        static void error(string msg, Exception e)
        {
            string printOut = "There was a problem " + msg + " the file. Error: " + e.Message;
            display(printOut);
        }

        static void success(string msg, string src, string dest)
        {
            string printOut = "Success " + msg + " the file from " + src + " to " + dest;
            display(printOut);
        }

        static void display(string printOut)
        {
            EditorUtility.DisplayDialog("Status", printOut, "OK");
            Debug.Log(printOut);
        }
    }
}
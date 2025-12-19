using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Voxon
{
    public class UnityBuildPreprocessor : IPreprocessBuildWithReport {

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {

           // Kill any Voxon Runtime that is running... 2024 - no need to do this anymore...
          //  Voxon.VXProcess.Runtime.Shutdown();
          //  Voxon.VXProcess.Runtime.Unload();

//            Voxon.VXProcess.Runtime.

            if (!AssetDatabase.IsValidFolder("Assets/StreamingAssets"))
            {
                Debug.LogError("You should never see this; Editor Handler should have fixed this already");

                System.IO.Directory.CreateDirectory("Assets/StreamingAssets");
                // InputController.SaveData(InputController.filename);
                Debug.LogWarning("Assets/StreamingAssets didn't exist. Created and Input File Saved (used loaded filename)");
            }
            else if(InputController.GetKey("Quit") == 0)
            {
                throw new BuildFailedException("Input controller requires 'Quit' to be bound (and saved)");
            }

            //EditorHandler.PrebuildMesh();
           // EditorHandler.PrebuildTextures();
        }
    }
}
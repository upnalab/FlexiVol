using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Voxon.Editor
{
    [CustomEditor(typeof(InputController))]
    public class InputControllerExt : UnityEditor.Editor {

      //  public RectTransform uiElement; 
        public static Vector2 ScrollPosition;
        public override void OnInspectorGUI()
        {

           // GUILayout.BeginArea(new Rect(10, 10, Screen.width - 200, Screen.height - 200));

            GUILayout.MaxHeight(2000); GUILayout.MinHeight(2000);
			GUILayout.MaxWidth(600); GUILayout.MinWidth(600);


            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(800), GUILayout.Height(600));

            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();

            /* -- TODO fix this layout
            GUILayout.Label("Some GUI Content Here");

            GUILayout.FlexibleSpace(); // Pushes the following controls to the bottom

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Pushes the button to the rightmost side
            */

            if (GUILayout.Button("New"))
            {
                InputController.Instance.keyboard.Clear();
                InputController.Instance.mouse.Clear();
                InputController.Instance.spacenav.Clear();
                InputController.Instance.j1Axis.Clear();
                InputController.Instance.j1Buttons.Clear();
                InputController.Instance.j2Axis.Clear();
                InputController.Instance.j2Buttons.Clear();
                InputController.Instance.j3Axis.Clear();
                InputController.Instance.j3Buttons.Clear();
                InputController.Instance.j4Axis.Clear();
                InputController.Instance.j4Buttons.Clear();
            }

            if (GUILayout.Button("Save")) InputController.SaveData();
            if (GUILayout.Button("Load")) InputController.LoadData();
            GUILayout.EndHorizontal();



           
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
}

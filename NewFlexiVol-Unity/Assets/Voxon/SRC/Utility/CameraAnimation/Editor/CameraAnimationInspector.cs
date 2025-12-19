using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraAnimation))]
public class CameraAnimationInspector : Editor
{
	/// <summary>
	/// Inspector for Camera Animation Objects.
	/// Handles displaying saved animation splines along with letting the user adjust their position in real time.
	/// </summary>
	
	// Internal Variables

	private static int selectedIndex = -1;
	static bool added = false;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	// Interaction Variables

	bool showRecording = true;
	bool showPlayback = true;
	SerializedProperty play_on_load;
	SerializedProperty record_on_load;
	SerializedProperty playback_file;
	SerializedProperty recording_file;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		CameraAnimation CA = target as CameraAnimation;

		showRecording = EditorGUILayout.BeginFoldoutHeaderGroup(showRecording, "Recording");

		if (showRecording)
		{
			EditorGUILayout.PropertyField(record_on_load);

			// 2 Colomns
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(recording_file);
			if (GUILayout.Button("Save Camera Animation"))
			{
				CA.SaveEditedPlaybackFile();
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndFoldoutHeaderGroup();
		showPlayback = EditorGUILayout.BeginFoldoutHeaderGroup(showPlayback, "Playback");

		if (showPlayback)
		{
			EditorGUILayout.PropertyField(play_on_load);

			// 2 Colomns
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(playback_file);
			if (GUILayout.Button("Load Camera Animation"))
			{
				CA.LoadPlaybackFile();
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndFoldoutHeaderGroup();

		EditorGUILayout.LabelField($"Frame: {CA.GetFrame()}", EditorStyles.boldLabel);
		EditorGUILayout.LabelField($"Length: {CA.GetAnimationLength()}", EditorStyles.boldLabel);
		if (CA.GetAnimationLength() > 0)
		{	
			UpdatePreviewFrame(CA, EditorGUILayout.IntSlider(CA.GetFrameIndex(), 0, CA.GetAnimationLength()-1));
		}

		serializedObject.ApplyModifiedProperties();

		EditorUtility.SetDirty(target);
	}

	private void UpdatePreviewFrame(CameraAnimation CA, int frame_index)
	{
		CA.SetFrame(frame_index);
		selectedIndex = frame_index;
		Repaint();
	}

	private void OnEnable()
	{
		play_on_load = serializedObject.FindProperty("play_on_load");
		record_on_load = serializedObject.FindProperty("record_on_load");
		playback_file = serializedObject.FindProperty("playback_file");
		recording_file = serializedObject.FindProperty("recording_file");

		if (!added)
		{


			//SceneView.onSceneGUIDelegate += this.UpdateSceneCallback;
			SceneView.duringSceneGui += this.UpdateSceneCallback;

			added = true;
		}
	}

	void OnDisable()
	{

		// Disable called when not active, not "disabled" in the hierarchy sense
		/*
		if (added)
		{
			Debug.Log("Disable-Removed");
			SceneView.onSceneGUIDelegate -= this.UpdateSceneCallback;
			added = false;
		}
		*/
		
		
	}
	void OnDestroy()
	{
		// Destroy is also called when not active
		/*
		if (added)
		{
			Debug.Log("Destroy-Removed");
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
			SceneView.onSceneGUIDelegate -= this.UpdateSceneCallback;
			added = false;
		}
		*/
	}

	void UpdateSceneCallback(SceneView sceneView)
	{
		OnSceneGUI();
	}

	void OnSceneGUI()
	{
		if (!added)
		{
			
			//SceneView.onSceneGUIDelegate += this.UpdateSceneCallback;
			SceneView.duringSceneGui += this.UpdateSceneCallback;

			added = true;
		}

		if(target == null || (target as CameraAnimation).gameObject.activeInHierarchy == false || EditorApplication.isPlaying)
		{
			//SceneView.onSceneGUIDelegate -= this.UpdateSceneCallback;
			SceneView.duringSceneGui -= this.UpdateSceneCallback;

			added = false;
			return;
		}

		CameraAnimation CA = target as CameraAnimation;
		
		if(CA.playback_frames != null)
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 15;
			style.normal.textColor = Color.red;

			for (int index = 0; index < CA.playback_frames.instants.Count; index++)
			{
				Vector3 point = CA.playback_frames.instants[index]._Position;

				if (selectedIndex == index)
				{
					Handles.color = modeColors[2];
					Quaternion rotation = CA.playback_frames.instants[index]._Rotation;
					float scale = CA.playback_frames.instants[index]._BaseScale;

					switch(Tools.current){
						case Tool.Move:
							{
								EditorGUI.BeginChangeCheck();
								point = Handles.DoPositionHandle(point, rotation);
								if (EditorGUI.EndChangeCheck())
								{
									// Undo.RecordObject(CA.playback_frames.instants[index], "Move Point");
									// EditorUtility.SetDirty(spline);
									CA.playback_frames.instants[index]._Position = point;
									EditorUtility.SetDirty(CA);
									Repaint();
								}
							}
							break;
						case Tool.Scale:
							{
								// TODO : Doesn't appear right :S
								EditorGUI.BeginChangeCheck();
								scale = Handles.DoScaleHandle(Vector3.one * CA.playback_frames.instants[index]._BaseScale, point, rotation, scale/10).x;
								if (EditorGUI.EndChangeCheck())
								{
									CA.playback_frames.instants[index]._BaseScale = scale ;
									EditorUtility.SetDirty(CA);
									Repaint();
								}
							}
							break;
						case Tool.Rotate:
							{
								// TODO : Doesn't appear right :S
								EditorGUI.BeginChangeCheck();
								rotation = Handles.DoRotationHandle(rotation, point);
								if (EditorGUI.EndChangeCheck())
								{
									// Undo.RecordObject(CA.playback_frames.instants[index], "Move Point");
									// EditorUtility.SetDirty(spline);
									CA.playback_frames.instants[index]._Rotation = rotation;
									EditorUtility.SetDirty(CA);
									Repaint();
								}
							}
							break;
						default:
							break;
					}
					// This handles GUI movement
					

					// Draw Superimposed image
					if(FindObjectOfType<VxViewFinder>() != null)
					{
						GameObject cam = FindObjectOfType<VxViewFinder>().gameObject;

						
						Mesh PreviewCam = cam.GetComponent<MeshFilter>().sharedMesh;
						Material PreviewMaterial = cam.GetComponent<MeshRenderer>().sharedMaterial;
						Vector3 PPos = CA.playback_frames.instants[index]._Position;
						Quaternion PRot = CA.playback_frames.instants[index]._Rotation;
						Vector3 PScale = CA.playback_frames.instants[index]._BaseScale * new Vector3(1, 0.4f, 1);
						Matrix4x4 PreviewMatrix = Matrix4x4.TRS(PPos, PRot, PScale);
						Graphics.DrawMesh(PreviewCam, PreviewMatrix, PreviewMaterial, 0);
					}
					else
					{
						Debug.Log("No ViewFinder Found");
					}

				}
				else
				{
					Handles.color = modeColors[0];
				}

				if(index > 0 && index < CA.playback_frames.instants.Count)
				{
					Handles.DrawLine(CA.playback_frames.instants[index-1]._Position, point);
				}
				

				if (Handles.Button(point, CA.playback_frames.instants[index]._Rotation, 0.02f, 0.02f, Handles.DotHandleCap)) {
					selectedIndex = index;
					EditorUtility.SetDirty(CA);
					Repaint();
				}

				
			}

			for (int index = 0; index < CA.playback_frames.instants.Count; index++)
			{
				if(selectedIndex == index)
				{
					style.normal.textColor = Color.red;
				} else
				{
					style.normal.textColor = Color.black;
				}
				Handles.Label(CA.playback_frames.instants[index]._Position, $"{CA.playback_frames.instants[index]._Frame}", style);
			}
		}
	}
}

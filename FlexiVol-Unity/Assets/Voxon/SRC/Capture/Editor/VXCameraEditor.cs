using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(VXCamera))]
[CanEditMultipleObjects]
public class VXCameraEditor : Editor
{
	SerializedProperty uniformScale;
	SerializedProperty baseScale;
	SerializedProperty vectorScale;


	SerializedProperty helixMode;
	SerializedProperty helixAspRMax;
	SerializedProperty loadViewFinder;
	SerializedProperty ViewFinderDimensions;

	bool showHelix = true;
	bool showScalars = true;
	bool showViewFinder = true;



	void OnEnable()
	{
		uniformScale = serializedObject.FindProperty("uniformScale");
		baseScale = serializedObject.FindProperty("baseScale");
		vectorScale = serializedObject.FindProperty("vectorScale");
		helixMode = serializedObject.FindProperty("helixMode");
		helixAspRMax = serializedObject.FindProperty("helixAspRMax");

		loadViewFinder = serializedObject.FindProperty("loadViewFinder");
		ViewFinderDimensions = serializedObject.FindProperty("ViewFinderDimensions");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();


		showHelix = EditorGUILayout.BeginFoldoutHeaderGroup(showHelix, "Display Type");
		if (showHelix)
		{
			EditorGUILayout.PropertyField(helixMode);
			if (helixMode.boolValue == true)
			{
				EditorGUILayout.PropertyField(helixAspRMax);
			}
		}

		EditorGUILayout.EndFoldoutHeaderGroup();

		showScalars = EditorGUILayout.BeginFoldoutHeaderGroup(showScalars, "Camera Scale");

		if (showScalars) { 
			EditorGUILayout.PropertyField(uniformScale);
			if (uniformScale.boolValue)
			{
				EditorGUILayout.PropertyField(baseScale);
			} else
			{
				EditorGUILayout.PropertyField(vectorScale);
			}
		}

		EditorGUILayout.EndFoldoutHeaderGroup();
		showViewFinder = EditorGUILayout.BeginFoldoutHeaderGroup(showViewFinder, "ViewFinder");

		if (showViewFinder) { 
			EditorGUILayout.PropertyField(loadViewFinder);
			if (!loadViewFinder.boolValue)
			{
				EditorGUILayout.PropertyField(ViewFinderDimensions);
			}
		}

		EditorGUILayout.EndFoldoutHeaderGroup();
		
		serializedObject.ApplyModifiedProperties();
	}
}
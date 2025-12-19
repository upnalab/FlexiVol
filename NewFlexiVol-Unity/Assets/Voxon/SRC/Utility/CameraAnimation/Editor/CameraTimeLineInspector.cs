using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraTimeLine))]
public class CameraTimeLineInspector : Editor
{
	private void OnSceneGUI()
	{
		CameraTimeLine CTL = target as CameraTimeLine;

		for(int i = 0; i < CTL.instants.Count; i++)
		{
			Handles.Button(CTL.instants[i]._Position, CTL.instants[i]._Rotation, 1, 2, Handles.DotHandleCap);
		}
	}

}

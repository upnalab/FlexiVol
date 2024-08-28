using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;

public class PythonTest : MonoBehaviour 
{
	void Start()
	{
		StartCoroutine(RunPythonRoutineFile());
	}

    IEnumerator RunPythonRoutineFile()
    {
    	PythonRunner.RunFile($"./Assets/Python/CreateBitPlanes.py");
    	yield return new WaitForEndOfFrame();
    	StartCoroutine(RunPythonRoutineFile());
    }
}

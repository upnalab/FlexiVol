// https://gist.github.com/ProGM/9cb9ae1f7c8c2a4bd3873e4df14a6687

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public static class PropertyDrawersHelper
{
#if UNITY_EDITOR
	static Voxon.MeshRegister mr = Voxon.MeshRegister.Instance;

	public static string[] AllSceneNames()
	{
		if (!Voxon.MeshRegister.Active)
		{
			mr.Enable();
		}

		string[] keys = mr.Keys();
		Array.Sort(keys, (x, y) => String.Compare(x, y));

		return keys;
	}

#endif
}
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraInstant
{
	public int _Frame;
	public Vector3 _Position;
	public Vector3 _ViewFinderDimensions;
	public Quaternion _Rotation;
	public float _BaseScale;
	

	public CameraInstant(int Frame, Vector3 Position, Quaternion Rotation, float BaseScale, Vector3 ViewFinderDimensions)
	{
		_Frame = Frame;
		_Position = Position;
		_Rotation = Rotation;
		_ViewFinderDimensions = ViewFinderDimensions;

		_BaseScale = BaseScale;
	}

	public string toJSON()
	{
		return JsonUtility.ToJson(this, true);
	}
}

public class CompCameraInstant : IComparer<CameraInstant>
{
	public int Compare(CameraInstant x, CameraInstant y)
	{
		if (x == null)
		{
			if (y == null)
			{
				return 0;
			}
			else
			{
				return 1;
			}

		}
		else
		{
			if (y == null)
			{
				return -1;
			}
			else
			{
				int difference = x._Frame.CompareTo(y._Frame);

				if (difference != 0)
				{
					return difference;
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
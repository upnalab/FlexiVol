using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class CameraTimeLine : ScriptableObject
{
	[SerializeField]
	public List<CameraInstant> instants;

	// [SerializeField]
	// public CameraInstant[] instantArr;

	int current_index = 0;

	float delta = 0.001f;
	public CameraTimeLine()
	{
		instants = new List<CameraInstant>();
	}

	public string ToJSON()
	{
		// instantArr = instants.ToArray();
		// Comparitor
		CompCameraInstant comp = new CompCameraInstant();
		instants.Sort(comp);

		return JsonUtility.ToJson(this);
	}

	public int AnimationLength
	{
		get
		{
			return instants.Count;
		}
	}

	public int FinalFrame
	{
		get
		{
			return instants[instants.Count-1]._Frame;
		}
	}

	public void AddInstant(CameraInstant ci)
	{
		// Base Case
		if (instants.Count == 0)
		{
			instants.Add(ci);
			return;
		}

		// Replace Case
		for (int i = 0; i < instants.Count; i++)
		{
			if(ci._Frame == instants[i]._Frame)
			{
				// Replace Case
				instants[i] = ci;
				return;
			} else if (ci._Frame > instants[i]._Frame)
			{
				// Pre Insert
				instants.Insert(i, ci);
				return;
			}
		}

		// Post Insert Case (Only Update for new frames, but may need to set a delta for pos / rot)
		CameraInstant lastInstant = instants[instants.Count];
		if (Vector3.Distance(ci._Position, lastInstant._Position) > delta ||
			Quaternion.Angle(ci._Rotation, lastInstant._Rotation) > delta ||
			Mathf.Abs(ci._BaseScale - lastInstant._BaseScale) > delta)
		{
			instants.Add(ci);
		}
		
	}

	public CameraInstant GetInstant(int frameNo)
	{
		// Error case so give empty value
		if(instants.Count == 0)
		{
			Debug.LogWarning("Requesting CameraInstant when none available!");
			new CameraInstant(0, Vector3.zero, Quaternion.identity, 1, new Vector3(1, 0.4f, 1));
		}

		if(frameNo == 0 && current_index > 0)
		{
			current_index = 0;
			return instants[current_index];
		}

		while(current_index < instants.Count)
		{
			// We want the current_index to be the 'next' frame in the list
			if(instants[current_index]._Frame <= frameNo)
			{
				current_index++;
			} else
			{
				break;
			}
		}

		if(current_index == 0)
		{
			return instants[current_index];
		} else
		{
			return instants[current_index-1];
		}
	}

	public int GetFrame(int index)
	{
		if(index < instants.Count)
		{
			return instants[index]._Frame;
		} else
		{
			return 0;
		}
	}
}

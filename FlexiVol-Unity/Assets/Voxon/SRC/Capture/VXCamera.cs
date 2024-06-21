using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class VXCamera : MonoBehaviour
{

	public bool uniformScale = true;
	[SerializeField]
	float baseScale = 1; // Need to adjust controls for this
	public Vector3 vectorScale = Vector3.one;
	[SerializeField]
	public bool helixMode = false;
	[SerializeField]
	public float helixAspRMax = 1.55f; // 1.55 for 45 cm x 15 ||  1.41 for 30 cm x 12 
	public bool loadViewFinder = false;

	public Vector3 ViewFinderDimensions = new Vector3(1, 0.4f, 1);

	VxViewFinder view_finder;
	Renderer vf_renderer;

	public CameraAnimation CameraAnimator;

	public float BaseScale
	{
		get
		{
			return baseScale;
		}

		set
		{
			baseScale = value;
			this.transform.hasChanged = true;
		}
	}

	void UpdatePerspective()
	{
		if (uniformScale){
			this.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
		} else {
			this.transform.localScale = vectorScale;
		}
	}

	void ViewFinderCheck()
	{
		if (view_finder == null)
		{
			view_finder = GetComponentInChildren<VxViewFinder>();

			// Handle Corrupted Prefab
			if (view_finder == null)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;

				// This value should be loaded from config data. Defaulting to current standard
				go.transform.localScale = ViewFinderDimensions;

				go.transform.parent = gameObject.transform;
				// Add a view finder
				go.name = "view_finder";
				go.AddComponent<VxViewFinder>();

			}
		}

		vf_renderer = view_finder.GetComponent<Renderer>();
	}

	void UpdateViewFinder()
	{
		ViewFinderCheck();

		view_finder.SetAspectRatio(ViewFinderDimensions);
	}

	private void OnEnable()
	{
		if (CameraAnimator == null)
		{
			CameraAnimator = gameObject.GetComponent<CameraAnimation>();
			if (CameraAnimator == null)
			{
				CameraAnimator = gameObject.AddComponent<CameraAnimation>();
			}
		}
	}

	private void Awake()
	{
		if (CameraAnimator == null)
		{
			CameraAnimator = gameObject.GetComponent<CameraAnimation>();
			if (CameraAnimator == null)
			{
				CameraAnimator = gameObject.AddComponent<CameraAnimation>();
			}
		}
#if UNITY_EDITOR
		// Load AspectRatio for ViewFinder
		UpdateViewFinder();
		UpdatePerspective();
#endif
	}
	// Start is called before the first frame update
	void Start()
    {

	}

// Update is called once per frame
void Update()
    {
		if (CameraAnimator == null)
		{
			CameraAnimator = gameObject.GetComponent<CameraAnimation>();
			if (CameraAnimator == null)
			{
				CameraAnimator = gameObject.AddComponent<CameraAnimation>();
			}
		}
#if UNITY_EDITOR
		UpdateViewFinder();
		UpdatePerspective();
#endif
	}

	public void LoadTransform()
	{
		CameraAnimator?.LoadTransform(this);
	}

	public void SaveTransform(bool hasChanged)
	{
		// TODO : Should handle non-uniform scales
		if (hasChanged)
		{
			CameraAnimator?.SaveTransform(transform, baseScale, ViewFinderDimensions);
			// Debug.Log(CameraAnimator?.name + " ; " + CameraAnimator.name);
		} else
		{
			CameraAnimator?.IncrementFrame();
		}
		
	}


	public void CloseAnimator()
	{
		CameraAnimator.StopPlayback();

		CameraAnimator.StopRecording();
		CameraAnimator.SaveRecording();
		
	}

	public Matrix4x4 GetMatrix()
	{
		ViewFinderCheck();

		return view_finder.transform.worldToLocalMatrix;
	}

	public Bounds GetBounds()
	{
		return vf_renderer.bounds;
	}
}

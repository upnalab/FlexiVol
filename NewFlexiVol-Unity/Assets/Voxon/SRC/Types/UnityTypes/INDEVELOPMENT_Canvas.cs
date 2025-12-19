using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class INDEVELOPMENT_Canvas : MonoBehaviour
{
    Canvas _canvas;
    Camera _UICamera;
    RenderTexture _RenderTexture;
    public GameObject _TargetPlane;
	public System.ValueTuple resolution;
    // Start is called before the first frame update
    void Start()
    {
        // Set Up Canvas
        _canvas = GetComponent<Canvas>();

        // Set Up Render Texture
        _RenderTexture = new RenderTexture(new RenderTextureDescriptor(1024,1024));

        // Set up UI Camera
        _UICamera = new Camera();
        // _UICamera.ClearFlags = ClearFlags.SolidColor;
        // _UICamera.Background = Color32.Black;
        // _UICamera.Projection = Orthgraphic;
        // _UICamera.TargetTexture = _RenderTexture;

        // Set Up Target Plane
        // Assign Render Texture to Plane
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

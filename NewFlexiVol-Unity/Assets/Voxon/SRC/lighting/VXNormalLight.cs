using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class VXNormalLight : MonoBehaviour
{
    public GameObject target;
    private Vector3 _lightDirection;

    public Light lightLight;

    [Header("Controls")]
    public Vector3 direction = Vector3.zero;
    public float intensity = 0;

    [Header("Display Settings")]
    public Mesh lightMesh;
    public Material lightMaterial;

	#region VoxonLightCode
	void Start()
    {
        if(target == null)
		{
            target = GetComponentInChildren<GameObject>();
            if(target == null)
			{
                GameObject go = new GameObject();
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
			}
		}

        // Force Update On Start if Playing (mainly required when testing in editor)
		if (Application.isPlaying)
		{
            Vector3 newVector = target.transform.position - transform.position;
            _lightDirection = newVector;
            Voxon.VXProcess.Instance.NormalLight = _lightDirection;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLight();
    }

    void UpdateLight()
	{
        Vector3 newVector = target.transform.position - transform.position;

        if (_lightDirection != newVector)
        {
            direction = _lightDirection.normalized;
            intensity = _lightDirection.magnitude;

            _lightDirection = newVector;
			if (Application.isPlaying)
			{
                Voxon.VXProcess.Instance.NormalLight = _lightDirection;
            }
        }
    }
    #endregion

    void OnValidate()
    {
        direction = direction.normalized;
        target.transform.localPosition = direction * intensity;
    }

    #region EditorDisplayMesh
    private void OnEnable()
    {

        Camera.onPreCull -= DrawWithCamera;
        Camera.onPreCull += DrawWithCamera;
    }

    private void OnDisable()
    {

        Camera.onPreCull -= DrawWithCamera;
    }

    private void DrawWithCamera(Camera camera)
    {
        if (camera && _lightDirection.magnitude != 0)
        {
            // Get central point
            Vector3 arrowPos = (target.transform.position + transform.position) * 0.5f;
            Quaternion arrowRot = Quaternion.LookRotation(_lightDirection, Vector3.forward);
            Vector3 arrowScale = Vector3.one * _lightDirection.magnitude * 0.2f;

			if (lightLight)
			{
                lightLight.transform.rotation = arrowRot;
            }
            // Debug.Log($"LDir: {lightDirection}, Pos: {arrowPos}, Rot: {arrowRot}, Scal: {arrowScale}");

            Draw(camera, Matrix4x4.TRS(arrowPos, arrowRot, arrowScale));
        }
    }

    private void Draw(Camera camera, Matrix4x4 matrix)
    {

        if (lightMesh && lightMaterial)
        {
            Graphics.DrawMesh(lightMesh, matrix, lightMaterial, gameObject.layer, camera);
        }
    }
    #endregion
}

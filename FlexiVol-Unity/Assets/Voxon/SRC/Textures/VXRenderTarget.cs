using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class VXRenderTarget : MonoBehaviour
{
    public RenderTexture renderTexture;
    [HideInInspector]
        public Texture2D _textureBuffer;
    private MeshRenderer _meshRenderer;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    VXComponent vxc;

    // Start is called before the first frame update
    void Start()
    {
        vxc = GetComponent<VXComponent>();
        _textureBuffer = new Texture2D((int)renderTexture.width, (int)renderTexture.height)
        {
            name = renderTexture.name,
        };
        // Assign Output Texture
        _meshRenderer = GetComponent<MeshRenderer>();
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (_meshRenderer != null)
        {
            _meshRenderer.material.mainTexture = _textureBuffer;
        }
        if (_skinnedMeshRenderer != null)
        {
            _skinnedMeshRenderer.material.mainTexture = _textureBuffer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_textureBuffer.width != renderTexture.width || _textureBuffer.height != renderTexture.height)
        {
            Debug.Log("Resize Texture");
            _textureBuffer.Reinitialize(renderTexture.width, renderTexture.height);
        }

        RenderTexture.active = renderTexture;
        _textureBuffer.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        _textureBuffer.Apply();
        RenderTexture.active = null;

        vxc.RefreshDynamicTexture(ref _textureBuffer);
    }
}

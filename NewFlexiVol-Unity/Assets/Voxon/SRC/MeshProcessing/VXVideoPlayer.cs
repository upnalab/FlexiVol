using UnityEngine;
using UnityEngine.Video;
using Voxon;

public class VXVideoPlayer : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    private VXComponent _vxc;

    private Texture2D _textureBuffer;

    private MeshRenderer _meshRenderer;

    private RenderTexture _renderTexture;
    // Start is called before the first frame update
    void Start()
    {
        // Generate Output Texture
        _videoPlayer = GetComponent<VideoPlayer>();
        
        
        _textureBuffer = new Texture2D((int) _videoPlayer.clip.width, (int) _videoPlayer.clip.height)
        {
            name = _videoPlayer.clip.name,
        };
        
        // Assign Output Texture
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material.mainTexture = _textureBuffer;
        
        // Configure Video Renderer
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.sendFrameReadyEvents = true;
        _videoPlayer.frameReady += RefreshTexture;
    }

    void RefreshTexture(VideoPlayer vp, long frameNo) //Following guide used at https://www.e-learn.cn/topic/2869730
    {
        if (_renderTexture == null && _videoPlayer.texture)
        {
            _renderTexture = _videoPlayer.texture as RenderTexture;
        }
        
        if (_vxc != null && _videoPlayer.texture) // Ensure VXComponent is present
        {
            if (_textureBuffer.width != _renderTexture.width || _textureBuffer.height != _renderTexture.height)
            {
                Debug.Log("Resize Texture");
                _textureBuffer.Reinitialize(_renderTexture.width, _renderTexture.height);
            }
            
            RenderTexture.active = _renderTexture;
            _textureBuffer.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            _textureBuffer.Apply();
            RenderTexture.active = null;
            
            _vxc.RefreshDynamicTexture(ref _textureBuffer);
        }
        else
        {
            _vxc = GetComponent<VXComponent>();
        }
        
    }

}

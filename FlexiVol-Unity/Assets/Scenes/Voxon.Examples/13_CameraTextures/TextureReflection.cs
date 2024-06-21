using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

namespace Voxon.Examples._13_TextureReflection { 
    /// <summary>
    /// Draw image from Camera to a texture and display this texture within the world.
    /// </summary>
    public class TextureReflection : MonoBehaviour
    {
        /// <summary>
        /// Camera used to capture scene
        /// </summary>
        public UnityEngine.Camera _camera;
        /// <summary>
        /// Texture that Camera data is written too
        /// </summary>
        private RenderTexture _renderTexture;
        /// <summary>
        /// Buffer to hold captured camera data
        /// </summary>
        private Texture2D _textureBuffer;
        /// <summary>
        /// Object the texture is applied to
        /// </summary>
        private MeshRenderer _meshRenderer;
        /// <summary>
        /// Component used to force the render texture data to be updated 
        /// in the Voxon Runtime
        /// </summary>
        private VXComponent _vxc;

        /// <summary>
        /// Called on Start.
        /// Assigns render texture from camera, generates color buffer to store texture data
        /// and assigns the mesh renderers texture to texture buffer.
        /// </summary>
        void Start()
        {

            _renderTexture = _camera.targetTexture;

            _textureBuffer = new Texture2D((int)_renderTexture.width, (int)_renderTexture.height)
            {
                name = _renderTexture.name,
            };

            // Assign Output Texture
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material.mainTexture = _textureBuffer;
        }

        /// <summary>
        /// Called per frame.
        /// Updates texture buffer with latest camera image, and makes the data available
        /// to VX Runtime
        /// </summary>
        // Update is called once per frame
        void Update()
        {
            if (_renderTexture == null && _camera.targetTexture)
            {
                _renderTexture = _camera.targetTexture;
            }

            if (_vxc != null && _camera.targetTexture) // Ensure VXComponent is present
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

        /// <summary>
        /// Called at end of frame update.
        /// Forces camera to render a frame (As automatic capture is disabled while graphics pipeline is off)
        /// </summary>
        private void LateUpdate()
        {
            _camera.Render();
        }
    }
}